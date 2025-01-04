module BunnyHop

open System
open System.Collections


let VERSION = "0.0.1"
printfn $"🐇 Bunny Hop v{VERSION}"


type Config = {
    inputBools: int
    outputs: int
    seed: int option
}

type History = {
    loss: array<float>
    accuracy: array<float>
}

type Outputs = array<byte>
type Activations = array<bool>

type Prediction = {
    intermediateActivations: array<Activations>
    outputActivations: Activations
} with

    member this.outputs = getOutputs this.outputActivations
    member this.result = maxOutputIndex this.outputs

type Evaluation = { isCorrect: bool; loss: float }

let evaluate (prediction: Prediction) (answer: int) : Evaluation = {
    isCorrect = prediction.result = answer
    loss = getLoss prediction.outputs answer
}

type Model = {
    bunnyHopVersion: string
    config: Config
    lastPrediction: Prediction
    lastAnswer: int
    history: History
}

let init (config: Config) : Model =
    // TODO
    let {
            inputBools = inputBoolsNb
            outputs = outputsNb
            seed = seed
        } =
        config


    // let billionParameters =
    //     (inputBoolsNb * layerNodesNb)
    //     + (layerNodesNb * layerNodesNb * (layersNb - 1))
    //     + (layerNodesNb * outputBoolsNb)
    //     |> float
    //     |> fun n -> n / 1_000_000_000.

    let rnd =
        match seed with
        | Some seed -> Random(seed)
        | None -> Random()

    let randomWeights (length: int) : NodeWeights =
        Array.init length (fun _ -> rnd.Next(-126, 127) |> sbyte)

    let randomLayerWeights (inputDim: int) (outputDim: int) : LayerWeights =
        Array.init outputDim (fun _ -> randomWeights inputDim)

    let model = {
        bunnyHopVersion = VERSION
        config = config
        lastPrediction = {
            intermediateActivations = Array.zeroCreate layersNb
            outputActivations = Array.zeroCreate outputBoolsNb
        }
        lastAnswer = -1
        history = { loss = [||]; accuracy = [||] }
    }

    printfn $"🐇 Bunny Hop model with {billionParameters} billion parameters ready."
    model

let predict (model: Model) (xs: Activations) : Prediction =
    // TODO

    {
        intermediateActivations = intermediateActivations
        outputActivations = outputActivations
    }

let teachModel (model: Model) (loss: float) (inputBools: Activations) (pred: Prediction) : unit =
    let { loss = previousLoss } = evaluate model.lastPrediction model.lastAnswer
    let isBetter = loss < previousLoss
    // TODO
    ()

type EpochData = { totalLoss: float; totalCorrect: int }

let rec fit
    (inputBoolsRows: array<Activations>)
    (labelIndexRows: array<int>)
    (epochs: int)
    (model: Model)
    : Model =
    if epochs < 1 then
        model
    else
        // let curr = model.history.loss.Length + 1
        // let total = model.history.loss.Length + epochs
        // let progress = String.replicate (12 * curr / total) "█"
        // let rest = String.replicate (12 - progress.Length) "░"
        // let progressBar = progress + rest
        // //! no way to update the same line https://stackoverflow.com/questions/47675136/is-there-a-way-to-update-the-same-line-in-f-interactive-using-printf
        // // printf "\u001b[1G"
        // // Console.SetCursorPosition(0, 0)
        // printfn
        //     $"Epoch {curr} of {total}\t {progressBar}\t Accuracy {100 * 0}%%\t Loss (MAE) {100 * 0}%%"

        model
// TODO
// let epochModel, epochData =
//     Array.fold2
//         rowFit
//         (model, { totalLoss = 0.0; totalCorrect = 0 })
//         inputBoolsRows
//         labelIndexRows

// fit inputBoolsRows labelIndexRows (epochs - 1) {
//     epochModel with
//         history = {
//             loss =
//                 Array.append model.history.loss [|
//                     epochData.totalLoss / float inputBoolsRows.Length
//                 |]
//             accuracy =
//                 Array.append model.history.accuracy [|
//                     (float epochData.totalCorrect) / float inputBoolsRows.Length
//                 |]
//         }
// }
