using OpenQA.Selenium;
using Shared;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Slave;



public class ConfigYoutobe
{
    public string ButtonIconSearch { get; set; } = "/html/body/ytd-app/div[1]/div/ytd-masthead/div[4]/div[2]/yt-icon-button/button";
    public string ButtonClearSearch { get; set; } = ".yt-spec-button-shape-next.yt-spec-button-shape-next--text.yt-spec-button-shape-next--mono.yt-spec-button-shape-next--size-m.yt-spec-button-shape-next--icon-only-default";
    public string InputSearch { get; set; } = "search_query";
    public string FirstVideo { get; set; } = "a.yt-simple-endpoint.style-scope.ytd-video-renderer";
    public string LikeVideo { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-watch-metadata/div/div[2]/div[2]/div/div/ytd-menu-renderer/div[1]/ytd-segmented-like-dislike-button-renderer/yt-smartimation/div/div[1]/ytd-toggle-button-renderer/yt-button-shape/button";
    public string SubscribeVideo { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-watch-metadata/div/div[2]/div[1]/div/ytd-subscribe-button-renderer/yt-smartimation/yt-button-shape/button";
    public string CheckSubcribeVideo { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-watch-metadata/div/div[2]/div[1]/div/ytd-subscribe-button-renderer/yt-smartimation/yt-button-shape/button/div/span";
    public string Commnent { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-comments/ytd-item-section-renderer/div[1]/ytd-comments-header-renderer/div[5]/ytd-comment-simplebox-renderer/div[1]";
    public string InputComment { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-comments/ytd-item-section-renderer/div[1]/ytd-comments-header-renderer/div[5]/ytd-comment-simplebox-renderer/div[3]/ytd-comment-dialog-renderer/ytd-commentbox/div[2]/div/div[2]/tp-yt-paper-input-container/div[2]/div/div[1]/ytd-emoji-input/yt-user-mention-autosuggest-input/yt-formatted-string/div";
    public string ButtonComment { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-comments/ytd-item-section-renderer/div[1]/ytd-comments-header-renderer/div[5]/ytd-comment-simplebox-renderer/div[3]/ytd-comment-dialog-renderer/ytd-commentbox/div[2]/div/div[4]/div[5]/ytd-button-renderer[2]/yt-button-shape/button";
    public string ChannelTabsRight { get; set; } = "a.yt-simple-endpoint.style-scope.ytd-compact-video-renderer";
    public string LinkTabsRight { get; set; } = "a.yt-simple-endpoint.style-scope.ytd-compact-video-renderer";
    public string ChannelCurrent { get; set; } = "/html/body/ytd-app/div[1]/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[2]/ytd-watch-metadata/div/div[2]/div[1]/ytd-video-owner-renderer/div[1]/ytd-channel-name/div/div/yt-formatted-string/a";
}
public class ConfigGoogle
{
    /// <summary>
    /// Input gmail Login
    /// </summary>
    public string InputEmailGoogle { get; set; } = "identifierId";
    /// <summary>
    /// Input password Login
    /// </summary>
    public string IntputPasswordGooogle { get; set; } = "Passwd";
    /// <summary>
    /// Nội dung "Đăng nhập" ở màn hình login
    /// </summary>
    public string CheckLogin { get; set; } = "a.gb_d.gb_La.gb_A";
    /// <summary>
    /// Element Chon tai khoan google
    /// </summary>
    public string ChooseUserGoogle { get; set; } = "li.JDAKTe.eARute.W7Aapd.zpCp3.SmR8";
}
public static class Extensions
{
    public static void WriteLine(string body, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(body);
        Console.ResetColor();
    }

    public static void ScrollTo(IWebDriver driver, int xPosition = 0, int yPosition = 0)
    {
        var js = System.String.Format("window.scrollTo({0}, {1})", xPosition, yPosition);
        ((IJavaScriptExecutor)driver).ExecuteScript(js);
    }

    public static IWebElement ScrollToView(IWebDriver driver, By selector)
    {
        var element = driver.FindElement(selector);
        ScrollToView(driver, element);
        return element;
    }

    public static void ScrollToView(IWebDriver driver, IWebElement element)
    {
        if (element.Location.Y > 200)
        {
            ScrollTo(driver, 0, element.Location.Y - 100); //Đảm bảo phần tử nằm trong dạng xem nhưng bên dưới ngăn dẫn hướng trên cùng
        }

    }
}

public static class Service
{
    static readonly Configuration config = Settings.Instance.Configuration;
    public static async Task<GmailUpdate> UpdateGmail(string profileId)
    {
        string apiUrl = $"{config.ApiBaseUrl}/CustomProfile/{profileId}";
        try
        {
            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
                {
                    Content = null
                };

                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    GmailUpdate responseData = JsonSerializer.Deserialize<GmailUpdate>(jsonString);

                    return responseData;
                }
                else
                {
                    Console.WriteLine("Request was not successful. Status code: " + response.StatusCode);
                    return null;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return null;
        }
    }
}