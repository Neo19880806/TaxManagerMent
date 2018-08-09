using SellerManagerment.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TaxManagerMent.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace TaxManagerMent
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private ObservableCollection<TaxSetting> mTypeList = new ObservableCollection<TaxSetting>();
        public SettingPage()
        {
            this.InitializeComponent();
            this.loadDefaultTypeList();
        }

        private void loadDefaultTypeList()
        {
            mTypeList.Clear();
            List<TaxSetting> resultList= SQLManager.DefalutInstance.Query();
            foreach(var li in resultList )
            {
                mTypeList.Add(li);
            }
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            string nameValue = nameTextBox.Text.Trim();
            if(nameValue=="")
            {
                await new MessageDialog("请输入分类名称").ShowAsync();
                nameTextBox.SelectAll();
                nameTextBox.Focus(FocusState.Programmatic);
            }
            else
            {
                TaxSetting tSetting = new TaxSetting();
                tSetting.TYPE = nameValue;
                bool bSuccess = SQLManager.DefalutInstance.Insert(tSetting);
                if(bSuccess)
                {
                    await new MessageDialog("添加成功").ShowAsync();
                    nameTextBox.Text = "";
                    nameTextBox.Focus(FocusState.Programmatic);
                    this.loadDefaultTypeList();
                }
                else
                {
                    await new MessageDialog("添加失败，请稍后重试").ShowAsync();
                    nameTextBox.SelectAll();
                    nameTextBox.Focus(FocusState.Programmatic);
                }
            }
        }

        private async void itemsTextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            bool bSuccess = SQLManager.DefalutInstance.Delete(dataListView.SelectedItem);
            if (bSuccess)
            {
                await new MessageDialog("删除成功").ShowAsync();
                this.loadDefaultTypeList();
            }
            else
            {
                await new MessageDialog("删除失败").ShowAsync();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
