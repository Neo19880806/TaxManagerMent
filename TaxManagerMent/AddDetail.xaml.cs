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
using Windows.UI.Text;
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
    public sealed partial class AddDetail : Page
    {
        private const int TYPE_ADD = 0;      //当前界面为'添加'界面
        private const int TYPE_VIEW = 1;      //当前界面为'查看'界面
        private const int TYPE_MODIFY = 2;      //当前界面为'修改'界面
        private TaxDetail mInitTaxDetail = null;
        private ObservableCollection<TaxSetting> mTypeList = new ObservableCollection<TaxSetting>();
        private int mType;
        public AddDetail()
        {
            this.InitializeComponent();
            this.loadDefaultTypeList();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(mInitTaxDetail!=null)
            {
                //恢复界面数据
                priceTextBox.Text = mInitTaxDetail.Price.ToString();
                typeComboBox.SelectedIndex = getPosition(mInitTaxDetail.Type);
                timeDatePicker.Date = Convert.ToDateTime(mInitTaxDetail.Time);
                commentsRichEditBox.Document.SetText(TextSetOptions.None, mInitTaxDetail.Comments);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TaxDetail)
            {
                mInitTaxDetail = (TaxDetail)e.Parameter;

                mType = TYPE_VIEW;
                switchSurfaceDisplay();
            }
            else
            {
                switchButton.Visibility = Visibility.Collapsed;
                mType = TYPE_ADD;
            }
            base.OnNavigatedTo(e);
}
        private void loadDefaultTypeList()
        {
            mTypeList.Clear();
            List<TaxSetting> resultList = SQLManager.DefalutInstance.Query();
            foreach (var li in resultList)
            {
                mTypeList.Add(li);
            }
        }

        private int getPosition(string type)
        {
            for(int i = 0;i < mTypeList.Count;i++)
            {
                if(mTypeList[i].TYPE == type)
                {
                    return i;
                }
            }
            return -1;
        }
        private async void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            //Tpye有效性
            if (typeComboBox.SelectedIndex == -1)
            {
                await new MessageDialog("请先选择分类").ShowAsync();
                typeComboBox.Focus(FocusState.Programmatic);
                return;
            }

            decimal price = 0m;
            try
            {
                price = decimal.Parse(priceTextBox.Text);
            }
            catch (Exception)
            {
                await new MessageDialog("请输入正确的价格").ShowAsync();
                priceTextBox.SelectAll();
                priceTextBox.Focus(FocusState.Programmatic);
                return;
            }

            //补充完整数据
            TaxDetail tDetail = new TaxDetail();
            tDetail.Type = ((TaxSetting)typeComboBox.SelectedItem).TYPE;
            tDetail.Price = price;
            tDetail.Time = timeDatePicker.Date.DateTime.ToString("yyyy-MM-dd HH:mm:ss");

            string comments = "";
            commentsRichEditBox.Document.GetText(TextGetOptions.None, out comments);
            tDetail.Comments = comments.Trim();

            string insertString = String.Format("\n类别：{0}\n价格：{1}\n日期：{2}\n",
                tDetail.Type,tDetail.Price,tDetail.Time);
            if (mType == TYPE_ADD)
            {
                //连接数据库，写入数据
                bool success = SQLManager.DefalutInstance.Insert(tDetail);
                if (success)//数据添加成功，清除数据
                {
                    await new MessageDialog(insertString, "信息添加成功").ShowAsync();
                    priceTextBox.Focus(FocusState.Programmatic);
                    ResetDetail();
                }
                else//失败
                {
                    var dialog = new MessageDialog("数据错误，请重新添加");
                    await dialog.ShowAsync();
                }
            }
            else//更新数据//以下内容待修改
            {
                //连接数据库，更新数据
                tDetail.Id = mInitTaxDetail.Id;
                bool success = SQLManager.getInstance().Update(tDetail);
                if (success)//数据修改成功，返回主界面，并更新列表
                {
                    await new MessageDialog(insertString,"信息修改成功").ShowAsync();
                    if (this.Frame.CanGoBack)
                    {
                        this.Frame.GoBack();
                    }
                }
                else//失败
                {
                    var dialog = new MessageDialog("数据错误，请重新修改");
                    await dialog.ShowAsync();
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void ResetDetail()
        {
            if(!lockTypeCheckBox.IsChecked.Value)
            {
                typeComboBox.SelectedIndex = -1;
            }

            if(!lockPriceCheckBox.IsChecked.Value)
            {
                priceTextBox.Text = "";
            }

            if(!lockTimeCheckBox.IsChecked.Value)
            {
                timeDatePicker.Date = DateTime.Now;
            }

            if(!lockCommentsCheckBox.IsChecked.Value)
            {
                commentsRichEditBox.Document.SetText(TextSetOptions.None, "");
            }
        }

        private async void lockTypeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (typeComboBox.SelectedIndex == -1)
            {
                await new MessageDialog("请选择正确的分类").ShowAsync();
                lockTypeCheckBox.IsChecked = false;
                return;
            }

            if (lockTypeCheckBox.IsChecked.Value)
            {
                typeComboBox.IsEnabled = false;
            }
            else
            {
                typeComboBox.IsEnabled = true;
            }
        }

        private async void lockPriceCheckBox_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0m;
            try
            {
                price = decimal.Parse(priceTextBox.Text);
            }
            catch (Exception)
            {
                await new MessageDialog("请输入正确的价格").ShowAsync();
                priceTextBox.SelectAll();
                priceTextBox.Focus(FocusState.Programmatic);
                lockPriceCheckBox.IsChecked = false;
                return;
            }

            if (lockPriceCheckBox.IsChecked.Value)
            {
                priceTextBox.IsEnabled = false;
            }
            else
            {
                priceTextBox.IsEnabled = true;
            }
        }

        private void lockTimeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (lockTimeCheckBox.IsChecked.Value)
            {
                timeDatePicker.IsEnabled = false;
            }
            else
            {
                timeDatePicker.IsEnabled = true;
            }
        }

        private void lockCommentsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (lockCommentsCheckBox.IsChecked.Value)
            {
                commentsRichEditBox.IsEnabled = false;
            }
            else
            {
                commentsRichEditBox.IsEnabled = true;
            }
        }

        private void switchSurfaceDisplay()
        {
            if (mType == TYPE_MODIFY)
            {
                //修改界面信息
                headTextBlock.Text = "修改客户信息";
                switchButton.Content = "查看";
                confirmButton.Visibility = Visibility.Visible;
                confirmButton.Content = "修改";

                typeComboBox.IsEnabled          = true;
                priceTextBox.IsEnabled          = true;
                timeDatePicker.IsEnabled        = true;
                commentsRichEditBox.IsEnabled   = true;

                lockTypeCheckBox.Visibility     = Visibility.Collapsed;
                lockPriceCheckBox.Visibility    = Visibility.Collapsed;
                lockTimeCheckBox.Visibility     = Visibility.Collapsed;
                lockCommentsCheckBox.Visibility = Visibility.Collapsed;
            }
            else if (mType == TYPE_VIEW)
            {
                //查看界面信息
                headTextBlock.Text = "查看客户信息";
                switchButton.Content = "修改";
                confirmButton.Visibility = Visibility.Collapsed;

                typeComboBox.IsEnabled          = false;
                priceTextBox.IsEnabled          = false;
                timeDatePicker.IsEnabled        = false;
                commentsRichEditBox.IsEnabled   = false;

                lockTypeCheckBox.Visibility     = Visibility.Collapsed;
                lockPriceCheckBox.Visibility    = Visibility.Collapsed;
                lockTimeCheckBox.Visibility     = Visibility.Collapsed;
                lockCommentsCheckBox.Visibility = Visibility.Collapsed;
            }
        }

        private void switchButton_Click(object sender, RoutedEventArgs e)
        {
            if (mType == TYPE_VIEW)
            {
                mType = TYPE_MODIFY;
            }
            else if (mType == TYPE_MODIFY)
            {
                mType = TYPE_VIEW;
            }
            //刷新界面
            switchSurfaceDisplay();
        }

    }
}
