using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Extensions.DateTimeExtensions;

namespace RedditSharp.Things
{
    public class Subreddit : Thing
    {
        private const string SubredditPostUrl = "/r/{0}.json";
        private const string SubredditNewUrl = "/r/{0}/new.json?sort=new";
        private const string SubredditHotUrl = "/r/{0}/hot.json";
        private const string SubredditRisingUrl = "/r/{0}/rising.json";
        private const string SubredditTopUrl = "/r/{0}/top.json?t={1}";
        private const string SubscribeUrl = "/api/subscribe";
        private const string GetSettingsUrl = "/r/{0}/about/edit.json";
        private const string GetReducedSettingsUrl = "/r/{0}/about.json";
        private const string ModqueueUrl = "/r/{0}/about/modqueue.json";
        private const string UnmoderatedUrl = "/r/{0}/about/unmoderated.json";
        private const string FlairTemplateUrl = "/api/flairtemplate";
        private const string ClearFlairTemplatesUrl = "/api/clearflairtemplates";
        private const string SetUserFlairUrl = "/api/flair";
        private const string StylesheetUrl = "/r/{0}/about/stylesheet.json";
        private const string UploadImageUrl = "/api/upload_sr_img";
        private const string FlairSelectorUrl = "/api/flairselector";
        private const string AcceptModeratorInviteUrl = "/api/accept_moderator_invite";
        private const string LeaveModerationUrl = "/api/unfriend";
        private const string BanUserUrl = "/api/friend";
        private const string UnBanUserUrl = "/api/unfriend";
        private const string AddModeratorUrl = "/api/friend";
        private const string AddContributorUrl = "/api/friend";
        private const string ModeratorsUrl = "/r/{0}/about/moderators.json";
        private const string FrontPageUrl = "/.json";
        private const string SubmitLinkUrl = "/api/submit";
        private const string FlairListUrl = "/r/{0}/api/flairlist.json";
        private const string CommentsUrl = "/r/{0}/comments.json";
        private const string SearchUrl = "/r/{0}/search.json?q={1}&restrict_sr=on&sort={2}&t={3}";
        private const string SearchUrlDate = "/r/{0}/search.json?q=timestamp:{1}..{2}&restrict_sr=on&sort={3}&syntax=cloudsearch";
        private const string ModLogUrl = "/r/{0}/about/log.json";
        private const string ContributorsUrl = "/r/{0}/about/contributors.json";
        private const string BannedUsersUrl = "/r/{0}/about/banned.json";
        private const string ModmailUrl = "/r/{0}/message/moderator/inbox.json";

        [JsonIgnore]
        private Reddit Reddit { get; set; }

        [JsonIgnore]
        private IWebAgent WebAgent { get; set; }

        [JsonIgnore]
        public Wiki Wiki { get; private set; }

        [JsonProperty("created")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? Created { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("description_html")]
        public string DescriptionHTML { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("header_img")]
        public string HeaderImage { get; set; }

        [JsonProperty("header_title")]
        public string HeaderTitle { get; set; }

        [JsonProperty("over_18")]
        public bool NSFW { get; set; }

        [JsonProperty("public_description")]
        public string PublicDescription { get; set; }

        [JsonProperty("subscribers")]
        public int? Subscribers { get; set; }

        [JsonProperty("accounts_active")]
        public int? ActiveUsers { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        [JsonConverter(typeof(UrlParser))]
        public Uri Url { get; set; }

        /// <summary>
        /// Property determining whether the current logged in user is a moderator on this subreddit.
        /// </summary>
        [JsonProperty("user_is_moderator")]
        public bool? UserIsModerator { get; set; }

        /// <summary>
        /// Property giving the moderator permissions of the logged in user on this subreddit.
        /// </summary>
        [JsonProperty("mod_permissions")]
        [JsonConverter(typeof(ModeratorPermissionConverter))]
        public ModeratorPermission ModPermissions { get; set; }

        /// <summary>
        /// Property determining whether the current logged in user is banned from the subreddit.
        /// </summary>
        [JsonProperty("user_is_banned")]
        public bool? UserIsBanned { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
        /// <summary>
        /// Top of the subreddit at a timeperiod
        /// </summary>
        /// <param name="timePeriod">Timeperiod you want to start at <seealso cref="FromTime"/></param>
        /// <returns>The top of the subreddit from a specific time</returns>
        public Listing<Post> GetTop(FromTime timePeriod)
        {
            if (Name == "/")
            {
                return new Listing<Post>(Reddit, "/top.json?t=" + Enum.GetName(typeof(FromTime), timePeriod).ToLower(), WebAgent);
            }
            return new Listing<Post>(Reddit, string.Format(SubredditTopUrl, Name, Enum.GetName(typeof(FromTime), timePeriod)).ToLower(), WebAgent);
        }
        /// <summary>
        /// All posts on a subredit
        /// </summary>
        public Listing<Post> Posts
        {
            get
            {
                if (Name == "/")
                    return new Listing<Post>(Reddit, "/.json", WebAgent);
                return new Listing<Post>(Reddit, string.Format(SubredditPostUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// Comments for a subreddit, all of them, irrespective of replies and what it is replying to
        /// </summary>
        public Listing<Comment> Comments
        {
            get
            {
                if (Name == "/")
                    return new Listing<Comment>(Reddit, "/comments.json", WebAgent);
                return new Listing<Comment>(Reddit, string.Format(CommentsUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// Posts on the subreddit/new
        /// </summary>
        public Listing<Post> New
        {
            get
            {
                if (Name == "/")
                    return new Listing<Post>(Reddit, "/new.json", WebAgent);
                return new Listing<Post>(Reddit, string.Format(SubredditNewUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// Posts on the front page of the subreddits
        /// </summary>
        public Listing<Post> Hot
        {
            get
            {
                if (Name == "/")
                    return new Listing<Post>(Reddit, "/.json", WebAgent);
                return new Listing<Post>(Reddit, string.Format(SubredditHotUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// List of rising posts
        /// </summary>
        public Listing<Post> Rising 
        {
            get 
            {
                if (Name == "/")
                    return new Listing<Post>(Reddit, "/.json", WebAgent);
                return new Listing<Post>(Reddit, string.Format(SubredditRisingUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// List of items in the mod queue
        /// </summary>
        public Listing<VotableThing> ModQueue
        {
            get
            {
                return new Listing<VotableThing>(Reddit, string.Format(ModqueueUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// Links a moderator hasn't checked
        /// </summary>
        public Listing<Post> UnmoderatedLinks
        {
            get
            {
                return new Listing<Post>(Reddit, string.Format(UnmoderatedUrl, Name), WebAgent);
            }
        }
        /// <summary>
        /// Search using specific terms from a specified time to now
        /// </summary>
        /// <param name="terms">Terms you want to search for</param>
        /// <param name="sortE">Sort the way you want to, see <see cref="Sorting"/></param>
        /// <param name="timeE">Time sorting you want to see</param>
        /// <returns>A list of posts</returns>
        public Listing<Post> Search(string terms, Sorting sortE = Sorting.Relevance, TimeSorting timeE = TimeSorting.All)
        {
            string sort = sortE.ToString().ToLower();
            string time = timeE.ToString().ToLower();

            return new Listing<Post>(Reddit, string.Format(SearchUrl, Name, Uri.EscapeUriString(terms), sort, time), WebAgent);
        }
        /// <summary>
        /// Search for a list of posts from a specific time to another time
        /// </summary>
        /// <param name="from">Time to begin search</param>
        /// <param name="to">Time to end search at</param>
        /// <param name="sortE">Sort of the objects you want to have it in</param>
        /// <returns>A list of posts in the range of time/dates in a specific order</returns>
        public Listing<Post> Search(DateTime from, DateTime to, Sorting sortE = Sorting.New)
        {
            string sort = sortE.ToString().ToLower();

            return new Listing<Post>(Reddit, string.Format(SearchUrlDate, Name, from.DateTimeToUnixTimestamp(), to.DateTimeToUnixTimestamp(), sort), WebAgent);
        }
        /// <summary>
        /// Settings of the subreddit, as best as possible
        /// </summary>
        public SubredditSettings Settings
        {
            get
            {
                if (Reddit.User == null)
                    throw new AuthenticationException("No user logged in.");
                try
                {
                    var request = WebAgent.CreateGet(string.Format(GetSettingsUrl, Name));
                    var response = request.GetResponse();
                    var data = WebAgent.GetResponseString(response.GetResponseStream());
                    var json = JObject.Parse(data);
                    return new SubredditSettings(this, Reddit, json, WebAgent);
                }
                catch // TODO: More specific catch
                {
                    // Do it unauthed
                    var request = WebAgent.CreateGet(string.Format(GetReducedSettingsUrl, Name));
                    var response = request.GetResponse();
                    var data = WebAgent.GetResponseString(response.GetResponseStream());
                    var json = JObject.Parse(data);
                    return new SubredditSettings(this, Reddit, json, WebAgent);
                }
            }
        }
        /// <summary>
        /// Get a list of the available user flair templates for the subreddit
        /// </summary>
        public UserFlairTemplate[] UserFlairTemplates
        {
            get
            {
                var request = WebAgent.CreatePost(FlairSelectorUrl);
                var stream = request.GetRequestStream();
                WebAgent.WritePostBody(stream, new
                {
                    name = Reddit.User.Name,
                    r = Name,
                    uh = Reddit.User.Modhash
                });
                stream.Close();
                var response = request.GetResponse();
                var data = WebAgent.GetResponseString(response.GetResponseStream());
                var json = JObject.Parse(data);
                var choices = json["choices"];
                var list = new List<UserFlairTemplate>();
                foreach (var choice in choices)
                {
                    UserFlairTemplate template = JsonConvert.DeserializeObject<UserFlairTemplate>(choice.ToString());
                    list.Add(template);
                }
                return list.ToArray();
            }
        }

        public SubredditStyle Stylesheet
        {
            get
            {
                var request = WebAgent.CreateGet(string.Format(StylesheetUrl, Name));
                var response = request.GetResponse();
                var data = WebAgent.GetResponseString(response.GetResponseStream());
                var json = JToken.Parse(data);
                return new SubredditStyle(Reddit, this, json, WebAgent);
            }
        }

        public IEnumerable<ModeratorUser> Moderators
        {
            get
            {
                var request = WebAgent.CreateGet(string.Format(ModeratorsUrl, Name));
                var response = request.GetResponse();
                var responseString = WebAgent.GetResponseString(response.GetResponseStream());
                var json = JObject.Parse(responseString);
                var type = json["kind"].ToString();
                if (type != "UserList")
                    throw new FormatException("Reddit responded with an object that is not a user listing.");
                var data = json["data"];
                var mods = data["children"].ToArray();
                var result = new ModeratorUser[mods.Length];
                for (var i = 0; i < mods.Length; i++)
                {
                    var mod = new ModeratorUser(Reddit, mods[i]);
                    result[i] = mod;
                }
                return result;
            }
        }

        public IEnumerable<TBUserNote> UserNotes
        {
            get
            {
                return ToolBoxUserNotes.GetUserNotes(WebAgent, Name);
            }
        }

        public Listing<Contributor> Contributors
        {
            get
            {
                return new Listing<Contributor>( Reddit, string.Format( ContributorsUrl, Name ), WebAgent );
            }
        }

        public Listing<BannedUser> BannedUsers
        {
            get
            {
                return new Listing<BannedUser>(Reddit, string.Format(BannedUsersUrl, Name), WebAgent);
            }
        }

        /// <summary>
        /// Subreddit modmail.
        /// <para/>
        ///  When calling <see cref="System.Linq.Enumerable.Take{T}"/> make sure to take replies into account!
        /// </summary>
        public Listing<PrivateMessage> Modmail
        {
            get
            {
                if (Reddit.User == null)
                    throw new AuthenticationException("No user logged in.");
                return new Listing<PrivateMessage>(Reddit, string.Format(ModmailUrl, Name), WebAgent);
            }
        }

        public async Task<Subreddit> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
        {
            CommonInit(reddit, json, webAgent);
            await JsonConvert.PopulateObjectAsync(json["data"].ToString(), this, reddit.JsonSerializerSettings);
            SetName();

            return this;
        }
        public Subreddit Init(Reddit reddit, JToken json, IWebAgent webAgent)
        {
            CommonInit(reddit, json, webAgent);
            JsonConvert.PopulateObject(json["data"].ToString(), this, reddit.JsonSerializerSettings);
            SetName();

            return this;
        }
        private void SetName()
        {
            Name = Url.ToString();
            if (Name.StartsWith("/r/"))
                Name = Name.Substring(3);
            if (Name.StartsWith("r/"))
                Name = Name.Substring(2);
            Name = Name.TrimEnd('/');
        }

        private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
        {
            base.Init(json);
            Reddit = reddit;
            WebAgent = webAgent;
            Wiki = new Wiki(reddit, this, webAgent);
        }
        /// <summary>
        /// http://www.reddit.com/r/all
        /// </summary>
        /// <param name="reddit">reddit, to help personalization</param>
        /// <returns>http://www.reddit.com/r/all</returns>
        public static Subreddit GetRSlashAll(Reddit reddit)
        {
            var rSlashAll = new Subreddit
            {
                DisplayName = "/r/all",
                Title = "/r/all",
                Url = new Uri("/r/all", UriKind.Relative),
                Name = "all",
                Reddit = reddit,
                WebAgent = reddit.WebAgent
            };
            return rSlashAll;
        }
        /// <summary>
        /// Gets the frontpage of the user
        /// </summary>
        /// <param name="reddit">Reddit you're logged into</param>
        /// <returns>the frontpage of reddit</returns>
        public static Subreddit GetFrontPage(Reddit reddit)
        {
            var frontPage = new Subreddit
            {
                DisplayName = "Front Page",
                Title = "reddit: the front page of the internet",
                Url = new Uri("/", UriKind.Relative),
                Name = "/",
                Reddit = reddit,
                WebAgent = reddit.WebAgent
            };
            return frontPage;
        }
        /// <summary>
        /// Subscribe to a subreddit
        /// </summary>
        public void Subscribe()
        {
            if (Reddit.User == null)
                throw new AuthenticationException("No user logged in.");
            var request = WebAgent.CreatePost(SubscribeUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                action = "sub",
                sr = FullName,
                uh = Reddit.User.Modhash
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            //Disposes and discards
        }
        /// <summary>
        /// Unsubscribes from a subreddit
        /// </summary>
        public void Unsubscribe()
        {
            if (Reddit.User == null)
                throw new AuthenticationException("No user logged in.");
            var request = WebAgent.CreatePost(SubscribeUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                action = "unsub",
                sr = FullName,
                uh = Reddit.User.Modhash
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            //Dispose and discard
        }

        public void ClearFlairTemplates(FlairType flairType)
        {
            var request = WebAgent.CreatePost(ClearFlairTemplatesUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                flair_type = flairType == FlairType.Link ? "LINK_FLAIR" : "USER_FLAIR",
                uh = Reddit.User.Modhash,
                r = Name
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void AddFlairTemplate(string cssClass, FlairType flairType, string text, bool userEditable)
        {
            var request = WebAgent.CreatePost(FlairTemplateUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                css_class = cssClass,
                flair_type = flairType == FlairType.Link ? "LINK_FLAIR" : "USER_FLAIR",
                text = text,
                text_editable = userEditable,
                uh = Reddit.User.Modhash,
                r = Name,
                api_type = "json"
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
        }

        public string GetFlairText(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_text"];
        }
        public async Task<string> GetFlairTextAsync(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_text"];
        }
        public string GetFlairCssClass(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_css_class"];
        }
        public async Task<string> GetFlairCssClassAsync(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_css_class"];
        }
        public void SetUserFlair(string user, string cssClass, string text)
        {
            var request = WebAgent.CreatePost(SetUserFlairUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                css_class = cssClass,
                text = text,
                uh = Reddit.User.Modhash,
                r = Name,
                name = user
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }
        public async Task SetUserFlairAsync(string user, string cssClass, string text)
        {
            var request = WebAgent.CreatePost(SetUserFlairUrl);
            var stream = await request.GetRequestStreamAsync();
            WebAgent.WritePostBody(stream, new
            {
                css_class = cssClass,
                text = text,
                uh = Reddit.User.Modhash,
                r = Name,
                name = user
            });
            stream.Close();
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }
        public void UploadHeaderImage(string name, ImageType imageType, byte[] file)
        {
            var request = WebAgent.CreatePost(UploadImageUrl);
            var formData = new MultipartFormBuilder(request);
            formData.AddDynamic(new
            {
                name,
                uh = Reddit.User.Modhash,
                r = Name,
                formid = "image-upload",
                img_type = imageType == ImageType.PNG ? "png" : "jpg",
                upload = "",
                header = 1
            });
            formData.AddFile("file", "foo.png", file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
            formData.Finish();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            // TODO: Detect errors
        }
        public async Task UploadHeaderImageAsync(string name, ImageType imageType, byte[] file)
        {
            var request = WebAgent.CreatePost(UploadImageUrl);
            var formData = new MultipartFormBuilder(request);
            formData.AddDynamic(new
            {
                name,
                uh = Reddit.User.Modhash,
                r = Name,
                formid = "image-upload",
                img_type = imageType == ImageType.PNG ? "png" : "jpg",
                upload = "",
                header = 1
            });
            formData.AddFile("file", "foo.png", file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
            formData.Finish();
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            // TODO: Detect errors
        }
        /// <summary>
        /// Adds a moderator
        /// </summary>
        /// <param name="user">User to add, by username</param>
        public void AddModerator(string user)
        {
            var request = WebAgent.CreatePost(AddModeratorUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "moderator",
                name = user
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }
        public void AddModerator(RedditUser user)
        {
            var request = WebAgent.CreatePost(AddModeratorUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "moderator",
                name = user.Name
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void AcceptModeratorInvite()
        {
            var request = WebAgent.CreatePost(AcceptModeratorInviteUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void RemoveModerator(string id)
        {
            var request = WebAgent.CreatePost(LeaveModerationUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "moderator",
                id
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public override string ToString()
        {
            return "/r/" + DisplayName;
        }

        public void AddContributor(string user)
        {
            var request = WebAgent.CreatePost(AddContributorUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "contributor",
                name = user
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void RemoveContributor(string id)
        {
            var request = WebAgent.CreatePost(LeaveModerationUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "contributor",
                id
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public async Task RemoveContributorAsync(string id)
        {
            var request = WebAgent.CreatePost(LeaveModerationUrl);
            WebAgent.WritePostBody(await request.GetRequestStreamAsync(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                type = "contributor",
                id
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        /// <summary>
        /// Bans a user
        /// </summary>
        /// <param name="user">User to ban, by username</param>
        /// <param name="reason">Reason for ban, shows in ban note as 'reason: note' or just 'note' if blank</param>
        /// <param name="note">Mod notes about ban, shows in ban note as 'reason: note'</param>
        /// <param name="duration">Number of days to ban user, 0 for permanent</param>
        /// <param name="message">Message to include in ban PM</param>
        public void BanUser(string user, string reason, string note, int duration, string message)
        {
            var request = WebAgent.CreatePost(BanUserUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                container = FullName,
                type = "banned",
                name = user,
                ban_reason = reason,
                note = note,
                duration = duration <= 0 ? "" : duration.ToString(),
                ban_message = message
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public async Task BanUserAsync(string user, string reason, string note, int duration, string message)
        {
            var request = WebAgent.CreatePost(BanUserUrl);
            WebAgent.WritePostBody(await request.GetRequestStreamAsync(), new
            {
                api_type = "json",
                uh = Reddit.User.Modhash,
                r = Name,
                container = FullName,
                type = "banned",
                name = user,
                ban_reason = reason,
                note = note,
                duration = duration <= 0 ? "" : duration.ToString(),
                ban_message = message
            });
            var response = await request.GetResponseAsync();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void BanUser(string user, string note)
        {
            BanUser(user, "", note, 0, "");
        }

        public async Task BanUserAsync(string user, string note)
        {
            await BanUserAsync(user, "", note, 0, "");
        }

        /// <summary>
        /// Unbans a user
        /// </summary>
        /// <param name="user">User to unban, by username</param>
        public void UnBanUser(string user)
        {
            var request = WebAgent.CreatePost(UnBanUserUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                uh = Reddit.User.Modhash,
                r = Name,
                type = "banned",
                container = FullName,
                executed = "removed",
                name = user,
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public async Task UnBanUserAsync(string user)
        {
            var request = WebAgent.CreatePost(UnBanUserUrl);
            WebAgent.WritePostBody(await request.GetRequestStreamAsync(), new
            {
                uh = Reddit.User.Modhash,
                r = Name,
                type = "banned",
                container = FullName,
                executed = "removed",
                name = user,
            });
            var response = await request.GetResponseAsync();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
        }

        private Post Submit(SubmitData data)
        {
            if (Reddit.User == null)
                throw new RedditException("No user logged in.");
            var request = WebAgent.CreatePost(SubmitLinkUrl);

            WebAgent.WritePostBody(request.GetRequestStream(), data);

            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(result);

            ICaptchaSolver solver = Reddit.CaptchaSolver;
            if (json["json"]["errors"].Any() && json["json"]["errors"][0][0].ToString() == "BAD_CAPTCHA"
                && solver != null)
            {
                data.Iden = json["json"]["captcha"].ToString();
                CaptchaResponse captchaResponse = solver.HandleCaptcha(new Captcha(data.Iden));

                // We throw exception due to this method being expected to return a valid Post object, but we cannot
                // if we got a Captcha error.
                if (captchaResponse.Cancel)
                    throw new CaptchaFailedException("Captcha verification failed when submitting " + data.Kind + " post");

                data.Captcha = captchaResponse.Answer;
                return Submit(data);
            }
            else if (json["json"]["errors"].Any() && json["json"]["errors"][0][0].ToString() == "ALREADY_SUB")
            {
                throw new DuplicateLinkException(string.Format("Post failed when submitting.  The following link has already been submitted: {0}", SubmitLinkUrl));
            }

            return new Post().Init(Reddit, json["json"], WebAgent);
        }
        private async Task<Post> SubmitAsync(SubmitData data)
        {
            if (Reddit.User == null)
                throw new RedditException("No user logged in.");
            var request = WebAgent.CreatePost(SubmitLinkUrl);

            WebAgent.WritePostBody(await request.GetRequestStreamAsync(), data);

            var response = await request.GetResponseAsync();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(result);

            ICaptchaSolver solver = Reddit.CaptchaSolver;
            if (json["json"]["errors"].Any() && json["json"]["errors"][0][0].ToString() == "BAD_CAPTCHA"
                && solver != null)
            {
                data.Iden = json["json"]["captcha"].ToString();
                CaptchaResponse captchaResponse = solver.HandleCaptcha(new Captcha(data.Iden));

                // We throw exception due to this method being expected to return a valid Post object, but we cannot
                // if we got a Captcha error.
                if (captchaResponse.Cancel)
                    throw new CaptchaFailedException("Captcha verification failed when submitting " + data.Kind + " post");

                data.Captcha = captchaResponse.Answer;
                return await SubmitAsync(data);
            }
            else if (json["json"]["errors"].Any() && json["json"]["errors"][0][0].ToString() == "ALREADY_SUB")
            {
                throw new DuplicateLinkException(string.Format("Post failed when submitting.  The following link has already been submitted: {0}", SubmitLinkUrl));
            }

            return new Post().Init(Reddit, json["json"], WebAgent);
        }
        /// <summary>
        /// Submits a link post in the current subreddit using the logged-in user
        /// </summary>
        /// <param name="title">The title of the submission</param>
        /// <param name="url">The url of the submission link</param>
        public Post SubmitPost(string title, string url, string captchaId = "", string captchaAnswer = "", bool resubmit = false)
        {
            return
                Submit(
                    new LinkData
                    {
                        Subreddit = Name,
                        UserHash = Reddit.User.Modhash,
                        Title = title,
                        URL = url,
                        Resubmit = resubmit,
                        Iden = captchaId,
                        Captcha = captchaAnswer
                    });
        }
        public async Task<Post> SubmitPostAsync(string title, string url, string captchaId = "", string captchaAnswer = "", bool resubmit = false)
        {
            return await
                SubmitAsync(
                    new LinkData
                    {
                        Subreddit = Name,
                        UserHash = Reddit.User.Modhash,
                        Title = title,
                        URL = url,
                        Resubmit = resubmit,
                        Iden = captchaId,
                        Captcha = captchaAnswer
                    });
        }

        /// <summary>
        /// Submits a text post in the current subreddit using the logged-in user
        /// </summary>
        /// <param name="title">The title of the submission</param>
        /// <param name="text">The raw markdown text of the submission</param>
        public Post SubmitTextPost(string title, string text, string captchaId = "", string captchaAnswer = "")
        {
            return
                Submit(
                    new TextData
                    {
                        Subreddit = Name,
                        UserHash = Reddit.User.Modhash,
                        Title = title,
                        Text = text,
                        Iden = captchaId,
                        Captcha = captchaAnswer
                    });
        }
        public async Task<Post> SubmitTextPostAsync(string title, string text, string captchaId = "", string captchaAnswer = "")
        {
            return await
                SubmitAsync(
                    new TextData
                    {
                        Subreddit = Name,
                        UserHash = Reddit.User.Modhash,
                        Title = title,
                        Text = text,
                        Iden = captchaId,
                        Captcha = captchaAnswer
                    });
        }
        /// <summary>
        /// Gets the moderation log of the current subreddit
        /// </summary>
        public Listing<ModAction> GetModerationLog()
        {
            return new Listing<ModAction>(Reddit, string.Format(ModLogUrl, this.Name), WebAgent);
        }
        /// <summary>
        /// Gets the moderation log of the current subreddit filtered by the action taken
        /// </summary>
        /// <param name="action">ModActionType of action performed</param>
        public Listing<ModAction> GetModerationLog(ModActionType action)
        {
            return new Listing<ModAction>(Reddit, string.Format(ModLogUrl + "?type={1}", Name, ModActionTypeConverter.GetRedditParamName(action)), WebAgent);
        }
        /// <summary>
        /// Gets the moderation log of the current subreddit filtered by moderator(s) who performed the action
        /// </summary>
        /// <param name="mods">String array of mods to filter by</param>
        public Listing<ModAction> GetModerationLog(string[] mods)
        {
            return new Listing<ModAction>(Reddit, string.Format(ModLogUrl + "?mod={1}", Name, string.Join(",", mods)), WebAgent);
        }
        /// <summary>
        /// Gets the moderation log of the current subreddit filtered by the action taken and moderator(s) who performed the action
        /// </summary>
        /// <param name="action">ModActionType of action performed</param>
        /// <param name="mods">String array of mods to filter by</param>
        /// <returns></returns>
        public Listing<ModAction> GetModerationLog(ModActionType action, string[] mods)
        {
            return new Listing<ModAction>(Reddit, string.Format(ModLogUrl + "?type={1}&mod={2}", Name, ModActionTypeConverter.GetRedditParamName(action), string.Join(",", mods)), WebAgent);
        }


        /// <summary>
        /// Infinitely yields new <see cref="Comment"/> posted to the subreddit.
        /// </summary>
        public IEnumerable<Comment> CommentStream
        {
            get
            {
                if (Name == "/")
                    return new Listing<Comment>(Reddit, "/comments.json", WebAgent).GetListingStream();
                return new Listing<Comment>(Reddit, string.Format(CommentsUrl, Name), WebAgent).GetListingStream();
            }
        }

        /// <summary>
        /// Infinitely yields new <see cref="Post"/> made to the subreddit.
        /// </summary>
        public IEnumerable<Post> SubmissionStream
        {
            get
            {
                if (Name == "/")
                    return new Listing<Post>(Reddit, "/new.json", WebAgent).GetListingStream();
                return new Listing<Post>(Reddit, string.Format(SubredditNewUrl, Name), WebAgent).GetListingStream();
            }
        }

        /// <summary>
        /// Infinitely yields new <see cref="ModAction"/> made on the subreddit.
        /// </summary>
        public IEnumerable<ModAction> ModerationLogStream
        {
            get
            {
                if (Name == "/")
                    return new Listing<ModAction>(Reddit, string.Format(ModLogUrl, this.Name), WebAgent).GetListingStream();
                return new Listing<ModAction>(Reddit, string.Format(ModLogUrl, this.Name), WebAgent).GetListingStream();
            }
        }

        #region Obsolete Getter Methods

        [Obsolete("Use Posts property instead")]
        public Listing<Post> GetPosts()
        {
            return Posts;
        }

        [Obsolete("Use New property instead")]
        public Listing<Post> GetNew()
        {
            return New;
        }

        [Obsolete("Use Hot property instead")]
        public Listing<Post> GetHot()
        {
            return Hot;
        }

        [Obsolete("Use ModQueue property instead")]
        public Listing<VotableThing> GetModQueue()
        {
            return ModQueue;
        }

        [Obsolete("Use UnmoderatedLinks property instead")]
        public Listing<Post> GetUnmoderatedLinks()
        {
            return UnmoderatedLinks;
        }

        [Obsolete("Use Settings property instead")]
        public SubredditSettings GetSettings()
        {
            return Settings;
        }

        [Obsolete("Use UserFlairTemplates property instead")]
        public UserFlairTemplate[] GetUserFlairTemplates() // Hacky, there isn't a proper endpoint for this
        {
            return UserFlairTemplates;
        }

        [Obsolete("Use Stylesheet property instead")]
        public SubredditStyle GetStylesheet()
        {
            return Stylesheet;
        }

        [Obsolete("Use Moderators property instead")]
        public IEnumerable<ModeratorUser> GetModerators()
        {
            return Moderators;
        }

        #endregion Obsolete Getter Methods
    }
}
