module Tools

open System.IO
open System

open Plotly.NET
open Plotly.NET.StyleParam
open Microsoft.DotNet.Interactive

open BunnyHop


type ByteRow = array<byte>

// parallel treatment of lines does NOT ensure the order of (label, data) pairs to be preserved
let loadMnistCsvUnsafeOrder (path: string) : array<int> * array<ByteRow> =
    File.ReadAllLines(path)
    // remove header row
    |> Array.skip 1
    |> Array.Parallel.reduceBy
        (fun row ->
            let columns = row.Split(",")
            let label = Array.head columns |> int

            let data =
                columns
                // remove label column
                |> Array.skip 1
                |> Array.map byte

            [| (label, data) |])
        Array.append
    |> Array.unzip

let byteToBoolRowsBinarized (threshold: byte) (byteRows: array<ByteRow>) : array<Activations> =
    byteRows |> Array.Parallel.map (Array.map (fun v -> v >= threshold))

// TODO
// let byteToBoolRows (byteRows: array<ByteRow>) : array<BitArray> =
//     byteRows |> Array.Parallel.map BitArray


let showRowDigitBinarized (threshold: byte) (row: ByteRow) : DisplayedValue =
    let image =
        row
        |> Array.Parallel.map (fun b ->
            let i = int b

            if threshold = 0uy then [| i; i; i |]
            else if b >= threshold then [| 255; 255; 255 |]
            else [| 0; 0; 0 |])
        |> Array.chunkBySize 28

    let chart =
        Chart.Image(image)
        |> Chart.withSize (100., 100.)
        |> Chart.withMarginSize (0., 0., 0., 0.)

    chart.Display()

let showRowDigit (row: ByteRow) : DisplayedValue = showRowDigitBinarized 0uy row

let showLayerWeights (title: string) (layerWeights: LayerWeights) : DisplayedValue =
    let xSize = 80 + max 300 (layerWeights.Length / 4)
    let ySize = max 300 (layerWeights[0].Length / 4)

    let chart =
        Chart.Heatmap(layerWeights, ColorScale = Colorscale.Picnic)
        |> Chart.withTitle title
        |> Chart.withXAxisStyle ("Inputs")
        |> Chart.withYAxisStyle ("Nodes")
        |> Chart.withColorBarStyle (TitleText = "Weights")
        |> Chart.withSize (xSize, ySize)
        |> Chart.withMarginSize (80., 10., 50., 10.)

    chart.Display()

let activationsArrayToMatrix (activationsArray: array<Activations>) : array<array<int>> =
    activationsArray
    |> Array.Parallel.map (Array.map (fun isActive -> if isActive then 1 else 0))

let showIntermediateActivations
    (title: string)
    (interActivations: array<Activations>)
    : DisplayedValue =
    let matrix = activationsArrayToMatrix interActivations

    let chart =
        Chart.Heatmap(
            zData = matrix,
            ColorScale = Colorscale.Blackbody,
            ShowScale = false,
            ReverseYAxis = true
        // rowNames = Array.map string [| 0 .. interActivations.Length - 1 |]
        )
        |> Chart.withTitle title
        |> Chart.withXAxisStyle ("Nodes")
        |> Chart.withYAxisStyle ("Layers")
        |> Chart.withColorBarStyle (TitleText = "Outputs")
        |> Chart.withSize (1000., 100. + 20. * float interActivations.Length)
        |> Chart.withMarginSize (80., 10., 50., 10.)

    chart.Display()

let showOutputs (title: string) (outputs: Outputs) : DisplayedValue =
    let intOutputs = Array.map int outputs
    let strLabels = Array.map string [| 0 .. outputs.Length - 1 |]

    let chart =
        Chart.Column(values = intOutputs, Keys = strLabels)
        |> Chart.withTitle title
        |> Chart.withXAxisStyle ("Digits")
        |> Chart.withYAxisStyle ("Values")
        |> Chart.withSize (1000., 250.)
        |> Chart.withMarginSize (80., 10., 50., 10.)

    chart.Display()

let showHistory (model: Model) : DisplayedValue =
    let accuracyChart =
        Chart.Line(
            xy = [
                for e in 1 .. model.history.accuracy.Length -> (e, model.history.accuracy[e - 1])
            ]
        )
        |> Chart.withTraceInfo (Name = "Accuracy")

    let lossChart =
        Chart.Line(
            xy = [ for e in 1 .. model.history.loss.Length -> (e, model.history.loss[e - 1]) ]
        )
        |> Chart.withTraceInfo (Name = "Loss")

    let chart =
        Chart.combine [ accuracyChart; lossChart ]
        |> Chart.withTitle $"Training History"
        |> Chart.withXAxisStyle ("Epochs")
        // |> Chart.withYAxisStyle ("Loss (MAE)")
        |> Chart.withSize (1000., 250.)
        |> Chart.withMarginSize (80., 10., 50., 10.)

    chart.Display()
