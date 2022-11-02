using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using phishingFilterBackend.GradientBoostTree;
using phishingFilterBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace phishingFilterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public static TransformerChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LinearBinaryModelParameters, PlattCalibrator>>> model = null;
        public static MLContext context = null;

        // GET: api/<ValuesController>
        [HttpGet]
        public ObjectResult Get()
        {
            trainModel();
            return Ok(new { estate = "on" });
        }

        // POST api/<ValuesController>
        [HttpPost]
        public ObjectResult Post(EmailMessage emailMessage)
        {
            if (context == null || model == null)
                trainModel();

            // configuro el motor de predicción y comienzo a predecir las probabilidades de phishing para los mensajes de prueba
            var predictionEngine = context.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(model);

            var messages = new SpamInput[] {
                new SpamInput() { Message = emailMessage.message }
            };

            // obtengo la predicción
            var myPredictions = from m in messages
                                select (Message: m.Message, Prediction: predictionEngine.Predict(m));

            // muestro los resultados
            foreach (var p in myPredictions)
            {
                float probability = p.Prediction.Probability;
                Console.WriteLine($"  [{probability:P2}] {p.Message}");

                return Ok(new { phishingProbability = Math.Round(probability * 100, 2) });
            }

            return Ok(new { phishingProbability = 0f });
        }

        private void trainModel()
        {
            context = new MLContext();

            // cargo el dataset en memoria
            LinkedList<SpamInput> inputs = TestData.getInputs();
            var data = context.Data.LoadFromEnumerable<SpamInput>(inputs);

            // uso 80% para el entrenamiento y 20% para pruebas
            var partitions = context.Data.TrainTestSplit(
                data,
                testFraction: 0.2);

            // configuro el canal de datos
            // paso 1: transformo 'spam' y 'ham'en true y false
            var pipeline = context.Transforms.CustomMapping<FromLabel, ToLabel>(
                mapAction: (input, output) => { output.Label = input.RawLabel == "spam" ? true : false; },
                contractName: "MyLambda")

                // paso 2: asigno el texto de entrada a analizar
                .Append(context.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(SpamInput.Message)))

                // paso 3: uso la clasificación del arbol de regresión binario con gradiente acelerada
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression());

            // pruebo la poda de hojas poco difusas de los arboles binarios para evitar el sobreajustamiento
            Console.WriteLine("Realizo la validación cruzada para evitar hojas poco difusas...");
            var cvResults = context.BinaryClassification.CrossValidate(
                data: partitions.TrainSet,
                estimator: pipeline,
                numberOfFolds: 5
            );

            // entreno el modelo con los mensajes de prueba
            model = pipeline.Fit(partitions.TrainSet);
        }
    }
}