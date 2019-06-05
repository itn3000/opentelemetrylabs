using System;
using System.Collections.Generic;
using OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Exporter.Zipkin;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Export;
using OpenTelemetry.Trace.Config;
using OpenTelemetry.Trace.Sampler;
using OpenTelemetry.Common;

namespace opentelemetrylabs.zipkin.console
{
    class MyTracer : ITracer
    {
        ISpan _Span;
        public ISpan CurrentSpan => _Span;

        public void RecordSpanData(ISpanData span)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder SpanBuilder(string spanName, SpanKind spanKind = SpanKind.Internal)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder SpanBuilderWithParent(string name, SpanKind kind = SpanKind.Internal, ISpan parent = null)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder SpanBuilderWithParentContext(string name, SpanKind kind = SpanKind.Internal, SpanContext parentContext = null)
        {
            throw new NotImplementedException();
        }

        public IScope WithSpan(ISpan span)
        {
            throw new NotImplementedException();
        }
    }
    class Program
    {
        static void ZipkinTest()
        {
            var exporter = new ZipkinTraceExporter(
                new ZipkinTraceExporterOptions()
                {
                    Endpoint = new Uri("http://172.16.12.254:9411"),
                    ServiceName = typeof(Program).Assembly.GetName().Name
                },
                OpenTelemetry.Trace.Tracing.ExportComponent
            );
            exporter.Start();
            var traceConfig = Tracing.TraceConfig;
            var traceParams = traceConfig.ActiveTraceParams;
            var newConfig = traceParams.ToBuilder()
                .SetSampler(Samplers.AlwaysSample)
                .Build();
            traceConfig.UpdateActiveTraceParams(newConfig);
            var tracer = Tracing.Tracer;
            var span = tracer.CurrentSpan;
            {
                var attrs = new Dictionary<string, IAttributeValue>()
                {
                    ["param1"] = AttributeValue.LongAttributeValue(128)
                };
                span.AddEvent("hogepiyo", attrs);
            }
            using (var scope = tracer.SpanBuilder("DoWork", SpanKind.Client).StartScopedSpan())
            {
                var attrs = new Dictionary<string, IAttributeValue>()
                {
                    ["param1"] = AttributeValue.LongAttributeValue(256)
                };
                tracer.CurrentSpan.AddEvent("hogepiyo", attrs);
            }
            Tracing.ExportComponent.SpanExporter.Dispose();

            Console.WriteLine("Hello World!");
        }
        static void Main(string[] args)
        {
            Samples.TestZipkin.Run("http://172.16.12.254:9411/api/v2/spans");
        }
    }
}
