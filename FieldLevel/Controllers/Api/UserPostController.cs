using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Cache;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FieldLevel.Models;

namespace FieldLevel.Controllers.Api
{
    public class UserPostController : ApiController
    {
        // Where the data is stored
        public static string uri = "https://jsonplaceholder.typicode.com/posts";

        // GET: api/UserPost
        public async Task<IEnumerable<UserPost>> Get()
        {
            var theUserPosts = await GetTheUserPosts();
            return theUserPosts;

            //return new string[] { "value1", "value2" };
        }

        //// GET: api/UserPost/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/UserPost
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/UserPost/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/UserPost/5
        //public void Delete(int id)
        //{
        //}

        #region Handler/Cache Policy
        static HttpRequestCachePolicy myPolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromMinutes(1));

        static WebRequestHandler myHandler = new WebRequestHandler()
        {
            CachePolicy = myPolicy
        };
        #endregion

        // The client to get the user posts
        static readonly HttpClient client = new HttpClient(myHandler);

        static async Task<IEnumerable<UserPost>> GetTheUserPosts()
        {
            // The list of posts to return
            IList<UserPost> resultingPosts = new List<UserPost>();

            try
            {
                // Get the data as a string
                string response = await client.GetStringAsync(uri);

                // Parse to JSON
                JArray array = JArray.Parse(response);

                // Keep track of the user ID
                string latestUserId = null;

                // Go through the posts
                foreach (JObject obj in array.Children<JObject>())
                {
                    // A single post                     
                    UserPost oneUserPost = new UserPost();

                    // Go through the properties of each post
                    foreach (JProperty singleProp in obj.Properties())
                    {
                        switch (singleProp.Name)
                        {
                            case "userId":
                                oneUserPost.UserId = singleProp.Value.ToString();
                                latestUserId = singleProp.Value.ToString();
                                break;
                            case "id":
                                oneUserPost.PostId = singleProp.Value.ToString();
                                break;
                            case "title":
                                oneUserPost.Title = singleProp.Value.ToString();
                                break;
                            case "body":
                                oneUserPost.Body = singleProp.Value.ToString();
                                break;
                            default:
                                break;
                        }
                    }

                    #region Next User                    
                    string theNextUserId = null;

                    // Get the next user ID
                    if (obj.Next != null)
                    {
                        theNextUserId = obj.Next["userId"].ToString();
                    }
                    #endregion

                    // Determine if this is the last user post in the list
                    if (latestUserId != theNextUserId)
                    {
                        // Add the post to the list
                        resultingPosts.Add(oneUserPost);
                    }
                }

                // Return the list
                return resultingPosts;
            }
            catch (HttpRequestException e)
            {
                // This needs to be improved
                return resultingPosts;
            }
        }
    }
}
