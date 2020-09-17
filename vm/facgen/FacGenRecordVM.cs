using FaceDetailsCreator.Data;
using FaceDetailsCreator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;

namespace FaceDetailsCreator.VM.Facgen
{
    class FacGenRecordVM: ViewModel
    {
        List<FacGenRecordData> _data;
        MBBindingList<FacGenRecordItemVM> _genRecordItemVMs;

        int MaxSaveCount = 50;

        BodyGeneratorView _generatorView;

        private FacGenRecordItemVM _lastSelectedItem;

        public FacGenRecordVM(BodyGeneratorView bodyGeneratorView, List<FacGenRecordData> data)
        {
            this._generatorView = bodyGeneratorView;
            this._data = data;
            if (null == this._genRecordItemVMs)
            {
                this._genRecordItemVMs = new MBBindingList<FacGenRecordItemVM>();
            }
            else
            {
                this._genRecordItemVMs.Clear();
            }


            if(null != this._data && this._data.Count > 0)
            {
                this._data.ForEach(obj => {
                    this._genRecordItemVMs.Add(new FacGenRecordItemVM(obj, OnSelectedItem));
                });
            }

        }

        [DataSourceProperty]
        public bool HasSelectedItem
        {
            get
            {
                return null != this._lastSelectedItem;
            }
        }


        [DataSourceProperty]
        public MBBindingList<FacGenRecordItemVM> RecordItems
        {
            get
            {
                return this._genRecordItemVMs;
            }
            set
            {
                if (value != this._genRecordItemVMs)
                {
                    this._genRecordItemVMs = value;
                    base.OnPropertyChanged("RecordItems");
                }
            }
        }

        public void OnSelectedItem(FacGenRecordItemVM item)
        {
            if(null != _lastSelectedItem)
            {
               this._lastSelectedItem.IsSelected = false;
            }
            this._lastSelectedItem = item;
            base.OnPropertyChanged("HasSelectedItem");
        }


        public void ExecuteSaveSelected()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("请输入保存名字").ToString(), new TextObject("最多只保存" + MaxSaveCount + "个").ToString(),
             true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(OnEnterNameAfter), InformationManager.HideInquiry, false));
        }

        public void OnEnterNameAfter(string saveName)
        {
            String propertiesString = this._generatorView.BodyGen.CurrentBodyProperties.ToString(); ;
            FacGenRecordData data = new FacGenRecordData(saveName, propertiesString);
            data.IsFemale = this._generatorView.BodyGen.IsFemale;
            if (this._data.Count >= MaxSaveCount)
            {
                this._data.RemoveAt(this._data.Count -1);
                this._genRecordItemVMs.RemoveAt(this._data.Count);
            }
            this._data.Insert(0, data);
            this._genRecordItemVMs.Insert(0, new FacGenRecordItemVM(data, OnSelectedItem));
            base.OnPropertyChanged("RecordItems");
            if(null != this._lastSelectedItem)
            {
                this._lastSelectedItem.IsSelected = false;
                this._lastSelectedItem = null;
            }
            base.OnPropertyChanged("HasSelectedItem");
        }

        public void ExecuteLoadSelected()
        {
            if (null != this._lastSelectedItem)
            {
                InformationUtils.ShowComfirInformation(new TextObject("是否确认加载选中"), null, () => {
                    String propertiesString = this._lastSelectedItem.GetFacGenRecordData().BodyPropertiesString;
                    BodyProperties bodyProperties = BodyProperties.Default;
                    BodyProperties.FromString(propertiesString, out bodyProperties);
                    this._generatorView.DataSource.SetBodyProperties(bodyProperties, !TaleWorlds.Core.FaceGen.ShowDebugValues, -1, true);
                    this._generatorView.DataSource.SelectedGender = this._lastSelectedItem.GetFacGenRecordData().IsFemale ? 1 : 0;
                });
            }
        }

    

        public void ExecuteDeleteSelected()
        {
            if (null != this._lastSelectedItem)
            {
                InformationUtils.ShowComfirInformation(new TextObject("警告: 是否删除选中"), new TextObject("请慎重选中，该操作将会删除数据，并且无法撤销"), () => {
                    this._lastSelectedItem.IsSelected = false;
                    this._data.Remove(this._lastSelectedItem.GetFacGenRecordData());
                    this._genRecordItemVMs.Remove(this._lastSelectedItem);
                    base.OnPropertyChanged("RecordItems");
                    this._lastSelectedItem = null;
                    base.OnPropertyChanged("HasSelectedItem");
                });

              
              
            }
        }

    }
}
