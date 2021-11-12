﻿using RetSim.Items;
using RetSim.Units.UnitStats;
using RetSimDesktop.Model;
using RetSimDesktop.View;
using RetSimDesktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RetSimDesktop
{
    /// <summary>
    /// Interaction logic for GearSlotSelect.xaml
    /// </summary>
    public partial class GearSlotSelect : UserControl
    {
        private static GearSim gearSimWorker = new();

        public int SlotID { get; set; }
        public IEnumerable<ItemDPS> SlotList
        {
            get => (IEnumerable<ItemDPS>)GetValue(SlotListProperty);
            set => SetValue(SlotListProperty, value);
        }

        public static readonly DependencyProperty SlotListProperty = DependencyProperty.Register(
            "SlotList",
            typeof(IEnumerable<ItemDPS>),
            typeof(GearSlotSelect));

        public List<Enchant> EnchantList
        {
            get => (List<Enchant>)GetValue(EnchantListProperty);
            set => SetValue(EnchantListProperty, value);
        }

        public static readonly DependencyProperty EnchantListProperty = DependencyProperty.Register(
            "EnchantList",
            typeof(List<Enchant>),
            typeof(GearSlotSelect));

        public ItemDPS SelectedItem
        {
            get => (ItemDPS)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(ItemDPS),
            typeof(GearSlotSelect));

        public Enchant SelectedEnchant
        {
            get => (Enchant)GetValue(SelectedEnchantProperty);
            set => SetValue(SelectedEnchantProperty, value);
        }

        public static readonly DependencyProperty SelectedEnchantProperty = DependencyProperty.Register(
            "SelectedEnchant",
            typeof(Enchant),
            typeof(GearSlotSelect));


        public GearSlotSelect()
        {
            InitializeComponent();
            this.DataContextChanged += (o, e) =>
            {
                if (DataContext is RetSimUIModel retSimUIModel)
                {
                    if(EnchantList == null)
                    {
                        EnchantComboBox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        EnchantComboBox.Visibility = Visibility.Visible;
                    }
                }
            };

            gearSlot.SetBinding(DataGrid.ItemsSourceProperty, new Binding("SlotList")
            {
                Source = this,
                Mode = BindingMode.OneWay,
            });

            gearSlot.SetBinding(DataGrid.SelectedItemProperty, new Binding("SelectedItem")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });

            EnchantComboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding("EnchantList")
            {
                Source = this,
                Mode = BindingMode.OneWay,
            });

            EnchantComboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding("SelectedEnchant")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });

            StatConverter statConverter = new();

            Binding strBinding = new("Item.Stats[" + StatName.Strength + "]");
            strBinding.Converter = statConverter;
            StrColumn.Binding = strBinding;
            Binding apBinding = new("Item.Stats[" + StatName.AttackPower + "]");
            apBinding.Converter = statConverter;
            APColumn.Binding = apBinding;
            Binding agiBinding = new("Item.Stats[" + StatName.Agility + "]");
            agiBinding.Converter = statConverter;
            AgiColumn.Binding = agiBinding;
            Binding critBinding = new("Item.Stats[" + StatName.CritRating + "]");
            critBinding.Converter = statConverter;
            CritColumn.Binding = critBinding;
            Binding hitBinding = new("Item.Stats[" + StatName.HitRating + "]");
            hitBinding.Converter = statConverter;
            HitColumn.Binding = hitBinding;
            Binding hasteBinding = new("Item.Stats[" + StatName.HasteRating + "]");
            hasteBinding.Converter = statConverter;
            HasteColumn.Binding = hasteBinding;
            Binding expBinding = new("Item.Stats[" + StatName.ExpertiseRating + "]");
            expBinding.Converter = statConverter;
            ExpColumn.Binding = expBinding;
            Binding apenBinding = new("Item.Stats[" + StatName.ArmorPenetration + "]");
            apenBinding.Converter = statConverter;
            APenColumn.Binding = apenBinding;
            Binding staBinding = new("Item.Stats[" + StatName.Stamina + "]");
            staBinding.Converter = statConverter;
            StaColumn.Binding = staBinding;
            Binding intBinding = new("Item.Stats[" + StatName.Intellect + "]");
            intBinding.Converter = statConverter;
            IntColumn.Binding = intBinding;
            Binding mp5Binding = new("Item.Stats[" + StatName.ManaPer5 + "]");
            mp5Binding.Converter = statConverter;
            MP5Column.Binding = mp5Binding;
            Binding spBinding = new("Item.Stats[" + StatName.SpellPower + "]");
            spBinding.Converter = statConverter;
            SPColumn.Binding = spBinding;
            Binding sCritBinding = new("Item.Stats[" + StatName.SpellCritRating + "]");
            sCritBinding.Converter = statConverter;
            SCritColumn.Binding = sCritBinding;
            Binding sHitBinding = new("Item.Stats[" + StatName.SpellHitRating + "]");
            sHitBinding.Converter = statConverter;
            SHitColumn.Binding = sHitBinding;
            Binding sHasteBinding = new("Item.Stats[" + StatName.SpellHasteRating + "]");
            sHasteBinding.Converter = statConverter;
            SHasteColumn.Binding = sHasteBinding;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGridCellTarget = (DataGridCell)sender;
            var header = dataGridCellTarget.Column.Header.ToString();

            Socket? selectedSocket = null;
            if (header == "Socket 1")
            {
                selectedSocket = SelectedItem.Item.Socket1;
            }
            else if (header == "Socket 2")
            {
                selectedSocket = SelectedItem.Item.Socket2;
            }
            else if (header == "Socket 3")
            {
                selectedSocket = SelectedItem.Item.Socket3;
            }

            if (selectedSocket != null)
            {
                GemPickerWindow gemPicker;
                if (selectedSocket.Color == SocketColor.Meta)
                {
                    gemPicker = new(RetSim.Data.Items.MetaGems.Values);
                }
                else
                {
                    gemPicker = new(RetSim.Data.Items.Gems.Values);
                }

                if (gemPicker.ShowDialog() == true)
                {
                    selectedSocket.SocketedGem = gemPicker.SelectedGem;
                    gearSlot.Items.Refresh();
                }
            }
        }

        private void GearSim_Click(object sender, RoutedEventArgs e)
        {
            if (!gearSimWorker.IsBusy && DataContext is RetSimUIModel retSimUIModel)
            {
                retSimUIModel.SimButtonStatus.IsGearSimButtonEnabled = false;
                gearSimWorker.RunWorkerAsync(new Tuple<RetSimUIModel, IEnumerable<ItemDPS>, int>(retSimUIModel, SlotList, SlotID));
            }
        }
    }
    public class StatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((float)value).ToString("0;;' '");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SocketConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not Socket socket)
            {
                return "-";
            }

            if (socket.SocketedGem != null)
            {
                return socket.SocketedGem.ID.ToString();
            }

            return socket.Color.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
