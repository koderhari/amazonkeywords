using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KeyGenerator
{
    class Program
    {

        static BlockingCollection<int> bc;
        static string txt = @"
Fire TV Cube is the first hands-free streaming media player with Alexa, delivering an all-in-one entertainment experience. From across the room, just ask Alexa to turn on the TV, dim the lights, and play what you want to watch.
With far-field voice recognition, eight microphones, and beamforming technology, Fire TV Cube hears you from any direction. Enjoy hands-free voice control of content—search, play, pause, fast forward, and more. Plus control the power and volume on your TV, sound bar, and A/V receiver as well as change live cable or satellite channels with just your voice.
Do more with Alexa. Fire TV Cube has a built-in speaker that lets you check the weather, listen to the news, control compatible smart home devices, and more—even with the TV off. Fire TV Cube is always getting smarter with new Alexa skills and voice functionality.
Experience true-to-life picture quality and sound with access to vivid 4K Ultra HD up to 60 fps, HDR, and the audio clarity of Dolby Atmos.
Enjoy tens of thousands of channels, apps, and Alexa skills. Get over 500,000 movies and TV episodes from Netflix, Prime Video, Hulu, HBO, SHOWTIME, NBC, and more.
Access YouTube, Facebook, Reddit, and more websites with Silk and Firefox browsers.
An Amazon Prime membership unlocks thousands of movies and TV episodes including ""Thursday Night Football"" and Prime Originals like “The Big Sick” and “Sneaky Pete”.
Enjoy unlimited access to tens of millions of songs with Amazon Music, starting at just $3.99/month.
Jump to: Compare Fire TV family | Technical details
We want you to know:
With Alexa on Fire TV Cube, you can control compatible TVs, sound bars, and A/V receivers from top brands like Samsung, Sony, LG, Vizio, and more.Plus, tune to live TV channels with cable or satellite boxes from providers like Comcast, DISH, DIRECTV/AT&T U-verse, and more. You cannot use your voice to change channels through an over-the-air HD antenna.Learn more about supported devices.

Use Alexa on Fire TV to control playback of content (play, pause, resume) in many of your favorite apps.Additionally, many apps including Netflix, Hulu, CBS All Access, SHOWTIME, NBC, and others have integrated even further with Alexa, which will allow you to browse, search, and change channels within supported apps. Voice control is getting smarter all the time.

Fire TV Cube will support sleep timers later this year.Alexa Calling & Messaging, multi-room music, and Bluetooth connections to mobile phones are not currently supported on Fire TV Cube. 
        ";
        private static HttpClient _single_httpClient;

        static void producer()
        {
            for (int i = 0; i < 100; i++)
            {
                bc.Add(i * i);
                Console.WriteLine("Производится число " + i * i);
            }
            //bc.CompleteAdding();
        }

        static void consumer()
        {
            int i;
            while (!bc.IsCompleted)
            {
                bc.Take();
                //if (bc.TryTake(out i))
                  //  Console.WriteLine("Потребляется число: " + i);
            }
        }


        private static void TestBC()
        {
            bc = new BlockingCollection<int>(4);

            // Создадим задачи поставщика и потребителя
            Task Pr = new Task(producer);
            Task Cn = new Task(consumer);
            Task.Run(() => { Task.Delay(5000).Wait(); bc.CompleteAdding(); });
            // Запустим задачи
            Pr.Start();
            Cn.Start();

            try
            {
                Task.WaitAll(Cn, Pr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Cn.Dispose();
                Pr.Dispose();
                bc.Dispose();
            }
        }



        public static async Task Main(string[] args)
        {
            //var r = new Rake("SmartStoplist.txt",3,2);
            //var tt=r.Run(txt);
            //TestBC();
            var urlTemplate = "https://completion.amazon.com/search/complete?mkt=1&l=en_US&sv=desktop&search-alias=aps&q={0}";
            Console.WriteLine();
            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = new WebProxy("191.242.177.169:21776", false),
            //    UseProxy = true
            //};
            //using (var client = new HttpClient(httpClientHandler))


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("UserAgent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                //var url = string.Format(urlTemplate, "tablet");
                var url = "https://www.amazon.com/Blue-Devils-Adult-Classic-Hoody/dp/B071V38NHF?pd_rd_wg=BdU8v&pd_rd_r=66ef0420-517f-4fa1-9608-9dafe54da58f&pd_rd_w=WqUcY&ref_=pd_gw_simh&pf_rd_r=JS6339TM8E7XNNTZECRK&pf_rd_p=b841581f-e864-5164-afa6-4c18a8348879";
                var response = client.GetAsync(url).Result;
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            using (var client = new HttpClient())
            {

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                //var url = string.Format(urlTemplate, "tablet");
                var url = string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode("big ball"));
                var response = client.GetAsync(url).Result;
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
            Console.WriteLine(((int)'a').ToString());
            Console.WriteLine(((int)'z').ToString());
            Console.WriteLine(((int)'0').ToString());
            Console.WriteLine(((int)'9').ToString());
            var words = @"suit
                          tablet
                          big ball
                            jacket
 jeans
mouse
laptop
computer game
warcraft
french parfume".Split("\r\n",StringSplitOptions.RemoveEmptyEntries);

            


            //var urls = GetUrls("tablet");
            var source = new ConcurrentQueue<string>();
            //var allUrls = new List<string>(370);
            //foreach (var word in words)
            //{

            //    var urls = GetUrls(word.Trim());
            //    foreach (var url in urls)
            //    {
            //        allUrls.Add(url);
            //        source.Enqueue(url);

            //    }

            //}
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var tasks = new List<Task>(370);
            _single_httpClient = new HttpClient();
            foreach (var word in words)
            {
                foreach (var url in GetUrlsEnum(word.Trim()))
                {
                    tasks.Add(Task.Run(() => ProccessAsync2(url)));
                }
            }


            //var tasks = allUrls.Select(x => ProccessAsync2(x)).ToArray(); 
            //var prox = GetProxies();
            //var tasks = prox.Select(x => ProccessAsync(x, source)).ToArray();
            //var tasks = allUrls.Select(x => Task.Run(() => ProccessAsync2(x))).ToArray();
            // var tasks = Enumerable.Range(1,10).Select(x => ProccessAsync3(source)).ToArray();
            //ForEachAsync(allUrls, 10, (x) => ProccessAsync2(x)).Wait();
            await Task.WhenAll(tasks);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);




            //Parallel.ForEach()


            Console.WriteLine("Hello World!");
            //return 1;
        }

        public static string[] GetProxies()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("http://www.freeproxy-list.ru/api/proxy?anonymity=false&token=demo").Result;
                var lists = response.Content.ReadAsStringAsync().Result;
                return lists.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public static Task ForEachAsync<T>(IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }

        public static string[] GetUrls(string queryWord)
        {
            var urlTemplate = "https://completion.amazon.com/search/complete?mkt=1&l=en_US&sv=desktop&search-alias=aps&q={0}";
            var urls = new string[37];
            var index = 0;
            urls[index] = string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode(queryWord));
            for (int i = (int)'a'; i <= (int)'z'; i++)
            {
                index++;
                urls[index] = string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }

            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                index++;
                urls[index] = string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }
            return urls;
        }

        public static IEnumerable<string> GetUrlsEnum(string queryWord)
        {
            var urlTemplate = "https://completion.amazon.com/search/complete?mkt=1&l=en_US&sv=desktop&search-alias=aps&q={0}";
            yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode(queryWord));

            for (int i = (int)'a'; i <= (int)'z'; i++)
            {
            
                yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }

            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                yield return string.Format(urlTemplate, System.Web.HttpUtility.UrlEncode($"{queryWord}{(char)i}"));
            }
            yield break;
        }

        public static async Task ProccessAsync(string proxy, ConcurrentQueue<string> source)
        {
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxy, false),
                UseProxy = true
            };
            using (var client = new HttpClient(httpClientHandler))
            {

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                try
                {
                    while (source.TryDequeue(out var url))
                    {
                        var response = await client.GetAsync(url);
                        //Console.WriteLine($"Response({proxy}): {url}" + response.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{proxy} {e}");
                    
                    
                }
               
            }
        }

        public static async Task ProccessAsync2(string url)
        {
            using (var client = new HttpClient())
            {

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                try
                {
                    
                    var response = await client.GetAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(url + result);
                    //Console.WriteLine($"Response({proxy}): {url}" + response.Content.ReadAsStringAsync().Result);

                }
                catch (Exception e)
                {
                    Console.WriteLine($" {e}");


                }

            }
        }

        public static async Task ProccessAsync4(string url)
        {
            

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                try
                {

                    var response = await _single_httpClient.GetAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(url + result);
                    //Console.WriteLine($"Response({proxy}): {url}" + response.Content.ReadAsStringAsync().Result);

                }
                catch (Exception e)
                {
                    Console.WriteLine($" {e}");


                }

          
        }

        public static async Task ProccessAsync3(ConcurrentQueue<string> source)
        {
            using (var client = new HttpClient())
            {

                //string apiUrl = "http://yourserver.com/postpage";
                //StringContent content = new System.Net.Http.StringContent("{dataelem:value}", Encoding.UTF8, "application/json");
                try
                {
                    while (source.TryDequeue(out var url))
                    {
                        var response = await client.GetAsync(url);
                        var result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response(): {url}" + result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($" {e}");


                }

            }
        }

        //      public async Task<MyResult> GetResult()
        //      {
        //          MyResult result = new MyResult();

        //          var tasks = Methods.Select(method => ProcessAsync(method)).ToArray();
        //          string[] json = await Task.WhenAll(tasks);

        //          result.Prop1 = PopulateProp1(json[0]);
        //...

        //return result;
        //      }

        //      pu
    }
}
