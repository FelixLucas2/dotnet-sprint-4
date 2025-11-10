using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Concurrent;

namespace Sprint03.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ml")]
    public class MlController : ControllerBase
    {
        private static readonly Lazy<PredictionEngine<TextInput, SentimentPrediction>> _engine = 
            new Lazy<PredictionEngine<TextInput, SentimentPrediction>>(Build);

        public record TextInput
        {
            [LoadColumn(0)] public string Text { get; set; } = string.Empty;
            [LoadColumn(1), ColumnName("Label")] public bool IsPositive { get; set; }
        }

        public class SentimentPrediction
        {
            [ColumnName("PredictedLabel")] public bool Prediction { get; set; }
            public float Probability { get; set; }
        }

        private static PredictionEngine<TextInput, SentimentPrediction> Build()
        {
            var ml = new MLContext(seed: 1);

            // Pequeno dataset em memória (somente para DEMO)
            var samples = new List<TextInput>
            {
                new TextInput { Text = "I love this product", IsPositive = true },
                new TextInput { Text = "This is fantastic", IsPositive = true },
                new TextInput { Text = "Excelente e barato", IsPositive = true },
                new TextInput { Text = "Horrible and slow", IsPositive = false },
                new TextInput { Text = "Terrible experience", IsPositive = false },
                new TextInput { Text = "péssimo suporte", IsPositive = false },
            };

            var data = ml.Data.LoadFromEnumerable(samples);
            var pipeline = ml.Transforms.Text.FeaturizeText("Features", nameof(TextInput.Text))
                .Append(ml.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            return ml.Model.CreatePredictionEngine<TextInput, SentimentPrediction>(model);
        }

        public record PredictRequest(string Text);

        [HttpPost("sentiment")]
        public ActionResult<object> Predict([FromBody] PredictRequest request)
        {
            var pred = _engine.Value.Predict(new TextInput { Text = request.Text });
            return Ok(new { label = pred.Prediction ? "positivo" : "negativo", probability = pred.Probability });
        }
    }
}
