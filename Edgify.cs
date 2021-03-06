﻿using System;
using Edgify;
using grpc = global::Grpc.Core;

namespace Edgify
{
    public class PredictionSdk
    {
        EdgifyService.EdgifyServiceClient client;
        AnalyticsServiceClient analytics_client;
        SamplesServiceClient samples_client;

        string host;
        int port;

        public PredictionSdk(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void Connect()
        {
            grpc.Channel channel = new grpc.Channel(host, port, grpc.ChannelCredentials.Insecure);
            this.client = new EdgifyService.EdgifyServiceClient(channel);
            this.analytics_client = new AnalyticsServiceClient(channel);
            this.samples_client = new SamplesServiceClient(channel);
        }

        public Prediction GetPrediction()
        {
            var request = new PredictionRequest();
            var response = client.GetPrediction(request);
            return response.Prediction;
        }

        public void CreateGroundTruth(Prediction prediction, string label, string source)
        {
            var groundTruth = new GroundTruth();
            groundTruth.Prediction = prediction;
            groundTruth.Label = label;
            groundTruth.Source = source;

            var request = new GroundTruthRequest();
            request.GroundTruth = groundTruth;
            var response = this.client.CreateGroundTruth(request);
        }
        
        public void DeleteSample(string uuid)
        {
            if !String.IsNullOrEmpty(uuid){
                this.samples_client.DeleteSample(uuid);
            }
        }

        public void StartCustomerTransaction()
        {
            this.analytics_client.CreateAnalyticsEvent("TransactionCustomerStart");
        }

        public void EndCustomerTransaction("TransactionCustomerStart")
        {
            this.analytics_client.CreateAnalyticsEvent("TransactionCustomerEnd");
        }
    }
}
