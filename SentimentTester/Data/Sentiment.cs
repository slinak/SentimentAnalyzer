using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentTester
{
    class Sentiment
    {
        public Probability probability;
        public string label;

        public static double GetProbabilityValue(Sentiment sent)
        {
            if (sent.label == "neg")
                return sent.probability.neg;
            else if (sent.label == "pos")
                return sent.probability.pos;
            else
                return sent.probability.neutral;
        }

        public static void PrintSentiment(Sentiment sent)
        {
            Console.WriteLine("----------------------------------");
            //Console.WriteLine(se);
            Console.WriteLine(sent.label);
            if (sent.label == "neg")
                Console.WriteLine(sent.probability.neg);
            if (sent.label == "pos")
                Console.WriteLine(sent.probability.pos);
            if (sent.label == "neutral")
                Console.WriteLine(sent.probability.neutral);
            Console.WriteLine("----------------------------------");
        }
    }

    
}
