using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace OgilvyOne.Common
{
    [Serializable]
    public class ResultData<T>
    {

        public ResultData()
        {
            Errors = new List<BusinessErrorEvent>();
        }

        public int ErrorCode { get; set; }
        public string ErrorHtml { get; set; }
        public bool HasError { get { return Errors.Count > 0; } set { } }
        public string Error
        {
            get
            {
                if (Errors.Count > 0)
                {
                    return Errors[Errors.Count - 1].Error;
                }
                return null;
            }
            set
            {
                Errors.Add(new BusinessErrorEvent(string.Empty, value));
            }
        }

        public T Data { get; set; }

        public IList<BusinessErrorEvent> Errors { get; set; }
        public void AddErrors(string field, string error)
        {
            Errors.Add(new BusinessErrorEvent(field, error));
        }

        public void Valid(System.Web.Mvc.ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count == 0) continue;
                    foreach (var err in item.Value.Errors)
                    {
                        this.AddErrors(item.Key, err.ErrorMessage);
                    }
                }
            }
        }
    }

    public class BusinessErrorEvent : EventArgs
    {
        public BusinessErrorEvent() { }
        public BusinessErrorEvent(string Key, string Error)
        {
            this.Key = Key;
            this.Error = Error;
        }

        public string Key { get; set; }
        public string Error { get; set; }
    }

}
