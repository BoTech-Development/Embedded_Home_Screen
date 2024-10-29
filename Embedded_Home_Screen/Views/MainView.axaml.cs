using Avalonia.Controls;
using Avalonia.Media;
using DialogHostAvalonia;
using Embedded_Home_Screen.ViewModels;
using Embedded_Home_Screen.ViewModels.Dialogs.Update;
using Material.Colors;
using Material.Icons.Avalonia;

namespace Embedded_Home_Screen.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel(this);
    }
    // All Methods which open a new Message Box:
    public async void ShowMsgBox_NewUpdateAvailable(NewUpdateAvailableDialogViewModel dialogViewModel)
    {
        Button updateBtn = new Button
        {
            Content = "UpdateNow",
            CommandParameter = "MSG_Box:NewUpdateAvailable;Param:UpdateNow",
            Margin = new Avalonia.Thickness(10, 0, 0, 0)
        };
        updateBtn.Click += HandleMsgBoxBtnClick;

        Button laterBtn = new Button
        {
            Content = "Later",
            CommandParameter = "MSG_Box:NewUpdateAvailable;Param:_Blank_",
            Margin = new Avalonia.Thickness(10, 0, 0, 0)
        };
        updateBtn.Click += HandleMsgBoxBtnClick;

        StackPanel content = new StackPanel()
        {
            Children =
            {
                new TextBlock
                {
                    Text = "A new Update is Available:",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    FontSize = 24
                },
                new MaterialIcon
                {
                    Kind = Material.Icons.MaterialIconKind.Update,
                    Width = 128,
                    Height = 128,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Foreground =  Brushes.Orange,
                },
                new Material.Styles.Controls.Card
                {
                    
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Your Version is: " + dialogViewModel.CurrentVersion,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,

                            },
                            new TextBlock
                            {
                                Text = "The Latest Version is: " + dialogViewModel.LatestVersion,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            },
                            new TextBlock
                            {
                                Text = "The version that will be installed: " + dialogViewModel.NextVersion,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            },
                        }
                    },
                   
                    
                },
                new TextBlock
                {
                     Text = "It could be the case that the latest version is not installed immediately. In this case, please press the \"Check for Updates\" button until the latest version has been installed.",
                     HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                },
                new TextBlock
                {
                    Text = "Would you like to perform the update now or at a later time?",
                     HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                },
                new StackPanel
                {
                    Children =
                    {
                        laterBtn,
                        updateBtn
                    },
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                }
            }
        };
        await DialogHost.Show(content, "MSG_Box:NewUpdateAvailable");
    }



    public void HandleMsgBoxBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button)
        {
            string Param = ((Button)sender).CommandParameter.ToString();
            if (Param != null)
            {
                string MsgBoxName = Param.Substring(0, Param.IndexOf(";"));
                MsgBoxName = MsgBoxName.Replace("MSG_Box:", "");
                string CommandParam = Param.Substring(Param.IndexOf(";") + 1);
                CommandParam = CommandParam.Replace("Param:", "");
                switch (MsgBoxName)
                {
                    case "NewUpdateAvailable":
                        DialogHost.Close("MSG_Box:NewUpdateAvailable");
                        if (CommandParam == "UpdateNow")
                        {
                            ((MainViewModel)DataContext).UpdateController.AplyUpdate();
                        }
                        break;

                }
            }
        }
    }
}
