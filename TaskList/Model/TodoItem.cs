using System;
using System.ComponentModel;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using TaskList.BaseClass;

namespace TaskList
{
	public enum LimitModeState
	{
		WithinLimit,
		DeadLine,
		OverDeadLine,
	}
    public class TodoItem : PropertyChangeBase
    {
        string id;
        string name;
        bool done;

        public TodoItem GetClone()
        {
            return (TodoItem)this.MemberwiseClone();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "text")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "complete")]
        public bool Done
        {
            get { return done; }
            set
            {
                done = value; 
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ButtonCaption));
            }
        }

        [JsonIgnore]
        public string DetailMenuText
        {
            get { return done ? "詳細表示" : "タスクを編集";}
        }
        private string userName;
		[JsonProperty(PropertyName = "UserName")]
		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		private int priority;
		[JsonProperty(PropertyName = "Priority")]
		public int Priority
		{
			get { return priority; }
			set 
            {
                priority = value;
                RaisePropertyChanged();
            }
		}

        private DateTime createdat;
		[JsonProperty(PropertyName = "createdAt")]
        public DateTime createdAt
		{
			get { return createdat; }
			set { createdat = value; }
		}

		private DateTime updatedat;
		[JsonProperty(PropertyName = "updatedAt")]
		public DateTime updatedAt
		{
			get { return updatedat; }
			set { updatedat = value; }
		}

		private DateTime completeDate;
		[JsonProperty(PropertyName = "CompleteDate")]
		public DateTime CompleteDate
		{
			get { return completeDate; }
			set { completeDate = value; }
		}

		private DateTime limitDate;
		[JsonProperty(PropertyName = "LimitDate")]
		public DateTime LimitDate
		{
			get { return limitDate; }
			set 
            {
                limitDate = value;
                RaisePropertyChanged(nameof(LimitMode));
                RaisePropertyChanged(nameof(DoingDateMsg));
            }
		}

		//ソート用
		[JsonIgnore]
		public DateTime SortDate
		{
            get { return LimitDate > new DateTime(1970,1,1) ? LimitDate : DateTime.MaxValue; }
		}

        private bool isSetLimit;
		[JsonProperty(PropertyName = "IsSetLimit")]
		public bool IsSetLimit
		{
			get { return isSetLimit; }
			set { isSetLimit = value; }
		}

		private bool isRegularTask;
		[JsonProperty(PropertyName = "IsRegularTask")]
		public bool IsRegularTask
		{
			get { return isRegularTask; }
			set { isRegularTask = value; }
		}



		private int regularTaskType;
		[JsonProperty(PropertyName = "RegularTaskType")]
		public int RegularTaskType
		{
			get { return regularTaskType; }
			set { regularTaskType = value; }
		}

		private string regularTaskData;
		[JsonProperty(PropertyName = "RegularTaskData")]
		public string RegularTaskData
		{
			get { return regularTaskData; }
			set { regularTaskData = value; }
		}

		[Version]
        public string Version { get; set; }

        [JsonIgnore]
        public LimitModeState LimitMode
        {
            get
            {
                int limitdateymd = 0;
                int nowdateymd = Done ? int.Parse(CompleteDate.ToString("yyyyMMdd")) : int.Parse(DateTime.Now.ToString("yyyyMMdd"));

                if (!IsSetLimit
                    || LimitDate < new DateTime(1900, 1, 1)
                    || (int.TryParse(limitDate.ToString("yyyyMMdd"), out limitdateymd) && limitdateymd > nowdateymd))
                    return LimitModeState.WithinLimit;
                
                if (limitdateymd == nowdateymd)
                {
                    return LimitModeState.DeadLine;
                }
                return LimitModeState.OverDeadLine;
            }
        }
        [JsonIgnore]
        public bool IsDeadLine
        {
            get { return LimitMode == LimitModeState.DeadLine; }
        }
		[JsonIgnore]
		public bool IsOverDeadLine
		{
			get { return LimitMode == LimitModeState.OverDeadLine; }
		}
        [JsonIgnore]
        public Action<object> ClickAction { get; set; }

		[JsonIgnore]
		public Action<object> StarClickAction { get; set; }

		[JsonIgnore]
		public Action<object> DeleteMenuClickAction { get; set; }

		[JsonIgnore]
		public Action<object> EditMenuClickAction { get; set; }

        [JsonIgnore]
        public string ButtonCaption { get { return Done ? "実行中に戻す" : "完了"; } }

        [JsonIgnore]
        public string DoingDateMsg
		{
			get 
            {
                string msg = string.Empty;
                if (Done)
                {
                    msg = CompleteDate > new DateTime(1900, 1, 1) ? "完了日時：" + CompleteDate.ToString("yyyy/MM/dd HH:mm") + "   " : string.Empty;

                }
                else
                {
					if (isRegularTask)
					{
                        msg = "繰り返し：";
                        switch (RegularTaskType)
                        {
                            case 0:
                                msg += "毎日"+ "   ";
                                break;
							case 1:
                                string week = string.Empty;
                                switch (regularTaskData)
                                {
                                    case "0":
                                        week = "日曜日";
                                        break;
									case "1":
										week = "月曜日";
										break;
									case "2":
										week = "火曜日";
										break;
									case "3":
										week = "水曜日";
										break;
									case "4":
										week = "木曜日";
										break;
									case "5":
										week = "金曜日";
										break;
									case "6":
										week = "土曜日";
										break;
                                    default:
                                        break;
                                }
                                msg += "毎週" + week + "   ";
								break;
                            case 2:
                                msg += "毎月" + regularTaskData + "日   ";
                                break;
                            default:
                                break;
                        }
					}
                    if(LimitDate > new DateTime(1900, 1, 1) && (IsSetLimit || IsRegularTask))
                    {
                        msg += "期限：" + LimitDate.ToString("yyyy/MM/dd") + "   ";
                    }
                }
                if(!string.IsNullOrEmpty(UserName))
                {
                    //変換方法を見直したい
                    msg += UserName == "yoshiaki" ? "担当者：義明" : UserName == "tomoko" ? "担当者：智ちゃん" : string.Empty;
                }
                return msg;
            }
		}
    }
}

