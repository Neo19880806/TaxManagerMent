using SellerManagerment.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaxManagerMent.Model;
using TaxManagerMent.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace TaxManagerMent
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<TaxDetail> mQueryList = new ObservableCollection<TaxDetail>();
        public MainPage()
        {
            this.InitializeComponent();
            this.loadDefaultTables();
        }

        private async void loadDefaultTables()
        {
            if (!SQLManager.DefalutInstance.checkTableExist())
            {
                bool initialSuccess = SQLManager.DefalutInstance.createTables("HelloWorld");
                var dialog = new MessageDialog("test");
                //数据库初始化
                if (initialSuccess)
                {
                    dialog.Content = "首次使用软件，数据库初始化成功";
                }
                else
                {
                    dialog.Content = "首次使用软件，数据库初始化失败";
                }
                await dialog.ShowAsync();
            }
        }

        private void controlsButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddDetail));
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            var Result = await new SearchDialog().ShowAsync();

            //等待对话框结束
            if (Result == ContentDialogResult.None)
            {
                refreshTable();
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {

            refreshTable();
        }

        private async void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (mQueryList.Count > 0)
            {
                var list = mQueryList.GroupBy(v=>v.Type)
                    .Select(v=> new{name=v.Key,count=v.Count(),total=v.Sum(t=>t.Price) });
                string strMessageString = "";
                foreach(var li in list)
                {
                    strMessageString += String.Format("类型：{0}\n记录数：{1}\t\t总价格：{2}\n\n",li.name,li.count,li.total.ToString("C"));
                }
                await new MessageDialog(strMessageString).ShowAsync();
            }
            else
            {
                await new MessageDialog("暂无数据可供统计").ShowAsync();
            }
        }

        private async void refreshTable()
        {
            RequestInfo request = dataManager.DefaultInstance.RequestInfo;
            if (request.Type != RequestInfo.TYPE_NONE)
            {
                List<TaxDetail> list = SQLManager.DefalutInstance.Query(request);
                mQueryList.Clear();

                if (list.Count > 0)
                {
                    foreach (var li in list)
                    {
                        mQueryList.Add(li);
                    }
                }
                else
                {
                    await new MessageDialog("未查询到相关信息").ShowAsync();
                }
            }
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void StackPanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var result = await new ContentDialog()
            {
                Title = "信息提示",
                Content = "查看数据详细内容？",
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消"
            }.ShowAsync();

            //选中项准备查看、修改
            if (result == ContentDialogResult.Primary)
            {
                this.Frame.Navigate(typeof(AddDetail), myListView.SelectedItem);
            }
        }

        private async void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StackPanel obj = (StackPanel)sender; 
            if(myListView.SelectedItem == null)
            {
                await new MessageDialog("请先选择项，然后删除").ShowAsync();
                return;
            }

            //删除数据提示框
            var result = await new ContentDialog()
            {
                Title = "信息提示",
                Content = "是否确认删除数据？",
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消"
            }.ShowAsync();

            //确认删除数据
            if (result == ContentDialogResult.Primary)
            {
                var tDetail = (TaxDetail)myListView.SelectedItem;
                bool success = SQLManager.getInstance().Delete(tDetail);

                var dialog = new MessageDialog("", "删除");
                if (success)
                {
                    dialog.Content = "删除信息成功！";
                    UpdateListBox(tDetail);
                }
                else
                {
                    dialog.Content = "删除信息失败";
                }
                await dialog.ShowAsync();
            }
        }

        //数据源删除指定ID数据
        private void UpdateListBox(TaxDetail tDetail)
        {
            mQueryList.Remove(tDetail);
        }

    }
}
