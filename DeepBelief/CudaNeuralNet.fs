﻿// The MIT License (MIT)
// 
// Copyright (c) 2014 SpiegelSoft Ltd
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace DeepBelief

module CudaNeuralNet =

    open DeepBeliefNet
    open CudaTemplates
    open Alea.CUDA
    open Alea.CUDA.Utilities
    open Utils

    let gpuComputeResults netProps trainingSet testSet nnEta nnAlpha rand epochs = 
        use runTrainNeuralNetEpochProgram = 32 |> runTrainNeuralNetEpochTemplate nnEta nnAlpha epochs |> Compiler.load Worker.Default
        let gpuOutput = runTrainNeuralNetEpochProgram.Run netProps rand trainingSet testSet
        let targets = testSet |> Array.map (fun x -> snd x)

        let testError = 
            Array.zip targets gpuOutput
            |> Array.fold (fun E (x, t) -> 
                let En = error t x
                E + En) 0.0f

        testError / (float32 testSet.Length)
