using System;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

//tone analyzer
//https://tone-analyzer-demo.mybluemix.net/
//docs: https://www.ibm.com/watson/developercloud/tone-analyzer/api/v3/#authentication

namespace SentimentTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //actions should be passed in args
            bool shouldImportStatements = false;
            bool shouldImportSentiments = false;
            bool shouldMakeRequest = false;

            string databaseName = "sentiment_tester";
            var connection = DBConnection.Instance();
            connection.DatabaseName = databaseName;

            List<string> statementTexts = new List<string>();
            List<Statement> statementMeta = new List<Statement>();

            if (connection.IsConnect() && shouldMakeRequest)
            {
                MySqlCommand cmd = connection.Connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM statement";

                MySqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                    statementMeta.Add(
                        new Statement {
                            statementID = reader.GetInt16(0),
                            content = reader.GetString(1),
                            date = reader.GetDateTime(2)
                        }
                    );

                reader.Close();

                foreach (var s in statementMeta)
                {
                    Sentiment sent = new JavaScriptSerializer().Deserialize<Sentiment>(MakeRequest(s.content));
                    s.sentiment = sent;
                    Sentiment.PrintSentiment(sent);
                }
            }
            
            if (connection.IsConnect() && shouldImportSentiments)
                foreach(var s in statementMeta)
                    DBConnection.PerformInsert(
                        connection, 
                        String.Format(
                            "INSERT INTO sentiment (label, statementID, pos, neg, neutral) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", 
                            s.sentiment.label, 
                            s.statementID, 
                            s.sentiment.probability.pos, 
                            s.sentiment.probability.neg, 
                            s.sentiment.probability.neutral
                        )
                    );


            if (connection.IsConnect() && shouldImportStatements)
            {
                foreach (var s in statementTexts)
                    DBConnection.PerformInsert(
                        connection, 
                        String.Format(
                            "INSERT INTO statement (content, date) VALUES('{0}', '{1}')", 
                            s, 
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        )
                    );
                Console.WriteLine("Statement Import Done");
            }

            Console.WriteLine("Press Enter to Continue...");
            Console.ReadLine();
        }
        
        public static string MakeRequest(string input)
        {
            string URI = "http://text-processing.com/api/sentiment/";
            string myParameters = "text=" + HttpUtility.UrlEncode(input);

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, myParameters);

                return HtmlResult;
            }
        }
    }
}
