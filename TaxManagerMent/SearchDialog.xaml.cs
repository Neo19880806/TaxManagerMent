using SellerManagerment.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaxManagerMent.Model;
using TaxManagerMent.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaxManagerMent
{
    public sealed partial class SearchDialog : ContentDialog
    {
        private ObservableCollection<TaxSetting> mTypeList = new ObservableCollection<TaxSetting>();
        public SearchDialog()
        {
            this.InitializeComponent();
            this.loadDefaultTypeList();
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
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void typeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            typeTextBlock.Visibility = Visibility.Visible;
            typeComboBox.Visibility = Visibility.Visible;

            startDateTextBlock.Visibility = Visibility.Collapsed;
            startDatePicker.Visibility = Visibility.Collapsed;

            endDateTextBlock.Visibility = Visibility.Collapsed;
            endDatePicker.Visibility = Visibility.Collapsed;
        }

        private void dateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            typeTextBlock.Visibility = Visibility.Collapsed;
            typeComboBox.Visibility = Visibility.Collapsed;

            startDateTextBlock.Visibility = Visibility.Visible;
            startDatePicker.Visibility = Visibility.Visible;

            endDateTextBlock.Visibility = Visibility.Visible;
            endDatePicker.Visibility = Visibility.Visible;
        }

        private void bothRadioButton_Click(object sender, RoutedEventArgs e)
        {
            typeTextBlock.Visibility = Visibility.Visible;
            typeComboBox.Visibility = Visibility.Visible;

            startDateTextBlock.Visibility = Visibility.Visible;
            startDatePicker.Visibility = Visibility.Visible;

            endDateTextBlock.Visibility = Visibility.Visible;
            endDatePicker.Visibility = Visibility.Visible;
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            //按类型搜索
            if(typeRadioButton.IsChecked.Value||bothRadioButton.IsChecked.Value)
            {
                //Tpye有效性
                if (typeComboBox.SelectedIndex == -1)
                {
                    await new MessageDialog("请先选择分类").ShowAsync();
                    typeComboBox.Focus(FocusState.Programmatic);
                    return;
                }

                dataManager.DefaultInstance.RequestInfo.Type = RequestInfo.TYPE_TYPE;
                dataManager.DefaultInstance.RequestInfo.Name = ((TaxSetting)typeComboBox.SelectedItem).TYPE;
            }

            //按日期搜索
            if(dateRadioButton.IsChecked.Value || bothRadioButton.IsChecked.Value)
            {
                DateTime startDate = startDatePicker.Date.DateTime;
                DateTime endDate = endDatePicker.Date.DateTime;
                dataManager.DefaultInstance.RequestInfo.Type = RequestInfo.TYPE_DATE;
                dataManager.DefaultInstance.RequestInfo.StartDate = startDate;
                dataManager.DefaultInstance.RequestInfo.EndDate = endDate;
            }

            if(bothRadioButton.IsChecked.Value)
            {
                dataManager.DefaultInstance.RequestInfo.Type = RequestInfo.TYPE_BOTH;
            }

            //搜索条件成立，关闭界面
            this.Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            dataManager.DefaultInstance.RequestInfo.Type = RequestInfo.TYPE_NONE;
            this.Hide();
        }
    }
}
