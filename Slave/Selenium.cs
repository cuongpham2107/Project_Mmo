using System;
using Mype.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
namespace Slave
{
    public class SeleniumHelper
    {
        ConfigGoogle _configGoole = new ConfigGoogle();

        ConfigYoutobe _configYoutobe = new ConfigYoutobe();

        IWebDriver driver;
        Actions actions;
        WebDriverWait wait => new WebDriverWait(driver, TimeSpan.FromMinutes(2));
        #region elementGoogle
        IWebElement emailTextBox => wait.Until(driver => driver.FindElement(By.Id(_configGoole.InputEmailGoogle)));
        IWebElement passWordTextBox => wait.Until(driver => driver.FindElement(By.Name(_configGoole.IntputPasswordGooogle)));
        IWebElement elementChooseUser => wait.Until(driver => driver.FindElement(By.ClassName(_configGoole.ChooseUserGoogle)));
        IWebElement elementCheckLogin => wait.Until(driver => driver.FindElement(By.CssSelector(_configGoole.CheckLogin)));
        #endregion
        #region element YouTobe
        private By elementButtonSearch => By.XPath(_configYoutobe.ButtonIconSearch);
        private By elementButtonClearSearch => By.CssSelector(_configYoutobe.ButtonClearSearch);
        private By elementYoutobeSearch => By.Name(_configYoutobe.InputSearch);
        private By elementFirstVideo => By.CssSelector(_configYoutobe.FirstVideo);
        private By elementLikeButton => By.XPath(_configYoutobe.LikeVideo);
        private By elementSubscribeButton => By.XPath(_configYoutobe.SubscribeVideo);
        private By elementInputComment => By.XPath(_configYoutobe.Commnent);
        private By elementInputCommentText => By.XPath(_configYoutobe.InputComment);
        private By elementButtonComment => By.XPath(_configYoutobe.ButtonComment);
        private By elementLinkTabRight => By.CssSelector(_configYoutobe.LinkTabsRight);
        private By elementChannelTabRight => By.CssSelector(_configYoutobe.ChannelTabsRight);
        private By elementChannelCurrent => By.XPath(_configYoutobe.ChannelCurrent);
        #endregion
        public SeleniumHelper(IWebDriver driver)
        {
            this.driver = driver;
            this.actions = new Actions(driver);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException), typeof(ElementNotInteractableException));
        }
        public void GoToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
            Extensions.WriteLine($"Go to url: {url}", ConsoleColor.Yellow);
        }
        #region Login Google
        public bool LoginGoogle(string email, string password)
        {
            string currentUrl = driver.Url;
            if (currentUrl.Contains("signin"))
            {
               
                try
                {
                    EnterEmail(email);
                    Thread.Sleep(3000);
                    EnterPassword(password);
                    if(driver.Url.Contains("recaptcha") || driver.Url.Contains("selection"))
                    {
                        return false;
                    }
                    else if (driver.Url.Contains("myaccount"))
                    {
                        Extensions.WriteLine("Đăng nhập Google thành công", ConsoleColor.DarkMagenta);
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                
                catch(Exception) { return false; }
            }
            else if (driver.Url.Contains("InteractiveLogin"))
            {
                
                try
                {
                    actions.MoveToElement(elementChooseUser).Click().Perform();
                    EnterEmail(email);
                    Thread.Sleep(3000);
                    EnterPassword(password);
                    if (driver.Url.Contains("recaptcha") || driver.Url.Contains("selection"))
                    {
                        return false;
                    }
                    else if (driver.Url.Contains("myaccount"))
                    {
                        Extensions.WriteLine("Đăng nhập Google thành công", ConsoleColor.DarkMagenta);
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (driver.Url.Contains("myaccount"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Nhập email ở màn hình đăng nhập Google
        /// </summary>
        /// <param name="email"></param>
        public void EnterEmail(string email)
        {
            try
            {
                actions.MoveToElement(emailTextBox).Click()
                    .KeyDown(Keys.Control)
                    .SendKeys("a")
                    .KeyUp(Keys.Control)
                    .SendKeys(Keys.Delete)
                    .Perform();
                Random r = new();
                foreach (var a in email)
                {
                    emailTextBox.SendKeys(a.ToString());
                    int pause = r.Next(100, 300);
                    Thread.Sleep(pause);
                }
                actions.SendKeys(emailTextBox, Keys.Enter).Perform();

            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {

                Extensions.WriteLine($"Lỗi: {ex.Message}", ConsoleColor.Red);
            }

        }
        /// <summary>
        /// Nhập password ở màn hình đăng nhập Google
        /// </summary>
        /// <param name="password"></param>
        public void EnterPassword(string password)
        {
            try
            {
                Extensions.ScrollToView(driver, passWordTextBox);
                Thread.Sleep(1000);
                actions.MoveToElement(passWordTextBox).Click().Perform();
                //passWordTextBox.Click();
                Random r = new();
                foreach (var a in password)
                {
                    passWordTextBox.SendKeys(a.ToString());
                    int pause = r.Next(300, 500);
                    Thread.Sleep(pause);
                }
                actions.SendKeys(passWordTextBox, Keys.Enter).Perform();


            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {

                Extensions.WriteLine($"Lỗi: {ex.Message}", ConsoleColor.Red);
            }

        }
        #endregion
        #region Watched video youtobe
        /// <summary>
        /// Tìm kiếm  video theo keyword
        /// </summary>
        /// <param name="keyword"></param>
        public void SearchVideo(string keyword)
        {
            try
            {
                IWebElement buttonSearch = wait.Until(driver => driver.FindElement(elementButtonSearch));
                actions.MoveToElement(buttonSearch).Click().Perform();
                IWebElement element = wait.Until(driver => driver.FindElement(elementYoutobeSearch));
                element.Clear();
                actions.MoveToElement(element).Click() // Nhấp vào phần tử để đảm bảo nó được chọn
                        .Perform();
                Random r = new();
                foreach (var a in keyword)
                {
                    element.SendKeys(a.ToString());
                    int pause = r.Next(100, 300);
                    Thread.Sleep(pause);
                }
                actions.SendKeys(element, Keys.Enter).Perform();
                Thread.Sleep(2000);
                ChooseVideo();
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {

                Extensions.WriteLine($"Lỗi: {ex.Message}", ConsoleColor.Red);
            }
        }
        public void ChooseVideo()
        {
            try
            {
                IWebElement element = wait.Until(driver => driver.FindElement(elementFirstVideo));
                actions.MoveToElement(element).Click().Perform();
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception e)
            {
                Extensions.WriteLine($"lỗi: {e}", ConsoleColor.Red);
            }
        }
        public void PlayingVideo(int timeToWatchVideo)
        {
            Thread.Sleep(TimeSpan.FromMinutes(timeToWatchVideo));
        }
        public void LikeVideo()
        {
            Random r = new Random();
            int yPst = 0;
            for (int i = 0; i < 5; i++)
            {
                yPst += 500;
                Extensions.ScrollTo(driver, xPosition: 0, yPosition: yPst);
                int pause = r.Next(500, 2000);
                Thread.Sleep(pause);
            }
            try
            {
                IWebElement element = wait.Until(driver => driver.FindElement(elementLikeButton));
                string classAttribute = element.GetAttribute("aria-pressed");
                if (classAttribute == "false")
                {
                    actions.MoveToElement(element).Click().Perform();
                    Extensions.WriteLine("Đã thích video thành công", ConsoleColor.Blue);
                }
                else
                {
                    Extensions.WriteLine("The video has been liked !!!", ConsoleColor.Yellow);
                }
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception e)
            {
                Extensions.WriteLine($"Lỗi: {e}", ConsoleColor.Red);
            }
        }
        public void SubVideo()
        {
            try
            {
                IWebElement element = wait.Until(driver => driver.FindElement(elementSubscribeButton));
                // Lấy văn bản trong nút "Subcribe"
                IWebElement checkSubcribe = wait.Until(driver => driver.FindElement(By.XPath(_configYoutobe.CheckSubcribeVideo)));

                if (string.Equals(checkSubcribe.Text, "Đăng ký", StringComparison.OrdinalIgnoreCase) == true)
                {
                    actions.MoveToElement(element).Click().Perform();
                    Extensions.WriteLine("Đã đăng ký video thành công", ConsoleColor.Blue);

                }
                else
                {
                    Extensions.WriteLine("Đã đăng ký channel rồi", ConsoleColor.Yellow);
                }
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (Exception e)
            {
                Extensions.WriteLine($"Lỗi: {e}", ConsoleColor.Red);
            }
        }
        public void CommentVideo(string[] comments, string[] icons)
        {
            Random r = new Random();
            int yPst = 0;
            for (int i = 0; i < 10; i++)
            {
                yPst += 500;
                Extensions.ScrollTo(driver, xPosition: 0, yPosition: yPst);
                int pause = r.Next(500, 2000);
                Thread.Sleep(pause);
            }

            if (comments.Count() > 0 || icons.Count() > 0)
            {

                int indexComment = r.Next(0, comments.Length);
                int indexIcon = r.Next(0, icons.Length);
                string randomComment = $"{comments[indexComment]} {icons[indexIcon]}";
                try
                {

                    IWebElement elementInput = wait.Until(driver => driver.FindElement(elementInputComment));


                    actions.MoveToElement(elementInput).Click().Perform();

                    IWebElement elementCommentText = wait.Until(driver => driver.FindElement(elementInputCommentText));


                    actions.MoveToElement(elementCommentText).Click() // Nhấp vào phần tử để đảm bảo nó được chọn
                           .KeyDown(Keys.Control)
                           .SendKeys("a")
                           .KeyUp(Keys.Control)
                           .SendKeys(Keys.Delete)
                           .Perform();
                    foreach (var a in randomComment)
                    {
                        elementCommentText.SendKeys(a.ToString());
                        int pause = r.Next(300, 500);
                        Thread.Sleep(pause);
                    }
                    IWebElement elementButton = wait.Until(driver => driver.FindElement(elementButtonComment));
                    actions.MoveToElement(elementButton).Click().Perform();
                    Extensions.WriteLine("Comment video thành công.", ConsoleColor.Blue);
                }
                catch (NoSuchElementException ex)
                {

                    Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
                }
                catch (TimeoutException ex)
                {

                    Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
                }
                catch (Exception e)
                {
                    Extensions.WriteLine($"Comment không thành công {e}", ConsoleColor.Red);
                }
            }
        }
        public bool NextVideoToUrl(string url)
        {
            try
            {
                List<IWebElement> elements = wait.Until(driver => driver.FindElements(elementLinkTabRight).ToList());
                foreach (IWebElement item in elements)
                {
                    string href = item.GetAttribute("href");
                    if (href.Contains(url))
                    {
                        actions.MoveToElement(item).Click().Perform();
                        Extensions.WriteLine("Đã chuyển sang xem video mới thành công", ConsoleColor.Blue);
                        return true;
                    }
                }
                return false;
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
                return false;
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
                return false;
            }
            catch (Exception e)
            {

                Extensions.WriteLine("Lỗi: " + e, ConsoleColor.Red);
                return false;
            }

        }
        public void NextVideoToChannel(string channel)
        {
            try
            {
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                //Current Channel
                IWebElement element = wait.Until(driver => driver.FindElement(elementChannelCurrent));
                string currentChannelName = (string)jsExecutor.ExecuteScript("return arguments[0].textContent;", element);

                //List Channel Tabs Right
                List<IWebElement> elements = wait.Until(driver => driver.FindElements(elementChannelTabRight)).ToList();

                foreach (WebElement item in elements)
                {
                    string channelName = (string)jsExecutor.ExecuteScript("return arguments[0].textContent;", item);

                    if (channelName.Contains(channel) == true)
                    {
                        Extensions.WriteLine($"Chuyển xem video tiếp theo của kênh {channel} thành công", ConsoleColor.Blue);
                        actions.MoveToElement(item).Click().Perform();
                        break;
                    }
                    else if (channelName.Contains(currentChannelName) == true)
                    {
                        Extensions.WriteLine($"Không tìm thấy kênh {channel}", ConsoleColor.Yellow);
                        Extensions.WriteLine($"Chuyển xem video tiếp theo của kênh {currentChannelName} thành công", ConsoleColor.Blue);
                        actions.MoveToElement(item).Click().Perform();
                        break;
                    }
                }
            }
            catch (NoSuchElementException ex)
            {

                Extensions.WriteLine($"Lỗi NoSuchElementException: {ex.Message}", ConsoleColor.Red);
            }
            catch (TimeoutException ex)
            {

                Extensions.WriteLine($"Lỗi TimeoutException: {ex.Message}", ConsoleColor.Red);
            }
            catch (System.Exception e)
            {
                Extensions.WriteLine($"Chuyển xem video tiếp thêm không thành công {e}", ConsoleColor.Red);
            }
        }
        #endregion 
    }
}