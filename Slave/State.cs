using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Slave
{
    public interface BaseState
    {
        Task Execute(Context context);
    }
    public class Context
    {
        private BaseState currentState;
        public Data Data { get; set; }
        public IWebDriver Driver;
        public ChromeOptions Options;
        public Context(IWebDriver Driver, Data data)
        {

            this.currentState = new LoggedOutGoogle();
            this.Driver = Driver;
            this.Data = data;
        }
        public void SetState(BaseState state)
        {
            currentState = state;
        }
        public async Task ExecuteState()
        {
            await currentState.Execute(this);
        }
        public void SetEmailPassword(string email, string password)
        {
            // Check if Data is not null before updating properties
            if (Data != null)
            {
                // Update the Email and Password properties
                Data.Email = email;
                Data.Password = password;
            }
        }

    }
    //public class Script : BaseState
    //{
    //    public async Task Execute(Context context)
    //    {
    //       List<BaseState> states = new List<BaseState>()
    //       {
    //           new LoggedOutGoogle(),
    //           new VideoSearchYoutobe(),
    //           new ChooseVideoYoutobe(),
    //           new PlayingVideoYoutobe(),
    //           new LikeVideoYoutobe(),
    //           new SubVideoYoutobe(),
    //           new CommentVideoYoutobe(),
    //           new NextVideoToUrlYoutobe(),
    //       };
    //        foreach (var state in states)
    //        {
    //            await state.Execute(context);
    //        }
    //    }
    //}
    public class LoggedOutGoogle : BaseState
    {
        public async Task Execute(Context context)
        {
            string email = context.Data.Email;
            string password = context.Data.Password;
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            s.GoToUrl("http://accounts.google.com");
            bool checkLogin = s.LoginGoogle(email, password);
            if (checkLogin)
            {
                context.SetState(new VideoSearchYoutobe());
            }
            else
            {
                //Post Api thong bao loi dang nhap google
                GmailUpdate newGmail = await Service.UpdateGmail(context.Data.ProfileId);
                if(newGmail  != null)
                {
                    context.SetEmailPassword(newGmail.email, newGmail.password);
                    context.SetState(new LoggedOutGoogle());
                }
                
            }
        }
    }
    public class VideoSearchYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            string[] keywords = context.Data.Keywords;

            var statesToExecute = new List<BaseState>
            {
                new ChooseVideoYoutobe(),
                new PlayingVideoYoutobe(),
                new LikeVideoYoutobe(),
                new SubVideoYoutobe(),
                new CommentVideoYoutobe(),
                context.Data.Script == 1 ? new NextVideoToUrlYoutobe() : new NextVideoToChannelYoutobe(),
                new NextVideoToUrlYoutobe(),
            };
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            s.GoToUrl("https://www.youtube.com");
            foreach (var keyword in keywords)
            {
                Extensions.WriteLine("Tìm kiếm từ khoá video", ConsoleColor.Blue);
                s.SearchVideo(keyword);
                foreach (var state in statesToExecute)
                {
                    await state.Execute(context);
                }
            }
        }
    }
    public class ChooseVideoYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            Extensions.WriteLine("Chọn video", ConsoleColor.Blue);
            s.ChooseVideo();
            context.SetState(new PlayingVideoYoutobe());
        }
    }
    public class PlayingVideoYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            int time = context.Data.TimeToWatchVideo;
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            Extensions.WriteLine("Xem video", ConsoleColor.Blue);
            s.PlayingVideo(time);
            context.SetState(new LikeVideoYoutobe());
        }
    }
    public class LikeVideoYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            s.LikeVideo();
            context.SetState(new SubVideoYoutobe());
        }
    }
    public class SubVideoYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            s.SubVideo();
            context.SetState(new CommentVideoYoutobe());
        }
    }
    public class CommentVideoYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            string[] comments = context.Data.Comments;
            string[] icons = context.Data.Icons;
            SeleniumHelper s = new SeleniumHelper(context.Driver);
            s.CommentVideo(comments, icons);
            context.SetState(new NextVideoToChannelYoutobe());
        }
    }
    public class NextVideoToUrlYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            string[] urls = context.Data.Urls;
            var statesToExecute = new List<BaseState>
            {
                new PlayingVideoYoutobe(),
                new LikeVideoYoutobe(),
                new SubVideoYoutobe(),
                new CommentVideoYoutobe()
            };
            foreach (var url in urls)
            {
                SeleniumHelper s = new SeleniumHelper(context.Driver);
                Extensions.WriteLine("Chọn video tiếp theo", ConsoleColor.Blue);
                bool check = s.NextVideoToUrl(url);
                if (check)
                {
                    foreach (var state in statesToExecute)
                    {
                        await state.Execute(context);
                    }
                }
                else
                {
                    Extensions.WriteLine($"Không tìm thấy video {url} này ", ConsoleColor.Yellow);
                }

            }
        }
    }
    public class NextVideoToChannelYoutobe : BaseState
    {
        public async Task Execute(Context context)
        {
            string[] channels = context.Data.Channels;
            var statesToExecute = new List<BaseState>
            {
                new PlayingVideoYoutobe(),
                new LikeVideoYoutobe(),
                new SubVideoYoutobe(),
                new CommentVideoYoutobe()
            };
            foreach (var channel in channels)
            {
                SeleniumHelper s = new SeleniumHelper(context.Driver);
                Extensions.WriteLine("Chon video tiep theo", ConsoleColor.Blue);
                s.NextVideoToChannel(channel);

                foreach (var state in statesToExecute)
                {
                    await state.Execute(context);
                }
            }
        }
    }
}

