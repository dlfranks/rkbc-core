using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewmodels
{
    public enum FormType { Standard };
    public enum FormViewMode { View, Edit, Create, List, Delete }
    public class FormActionViewModel
    {
        string _controller;
        int _id;

        string _index;
        string _delete;
        string _details;
        string _edit;
        string _create;
        

        public FormActionViewModel(Controller controller, int id, FormViewMode mode, FormType type = FormType.Standard, String queryString = "")
        {
            formViewMode = mode;
            formType = type;
            _controller = controller.ControllerContext.RouteData.Values["controller"].ToString();
            _id = id;
            this.queryString = queryString;
        }

        public FormType formType { get; set; }
        public FormViewMode formViewMode { get; set; }

        protected string urlConstruct(string controller, string action)
        {
            var qs = String.IsNullOrEmpty(queryString) ? "" : ("?" + queryString);
            return ("/" + controller + "/" + action + "/" + id + qs);
        }
        protected string urlCreate(string action)
        {
            var qs = String.IsNullOrEmpty(queryString) ? "" : ("?" + queryString);
            return ("/" + _controller + "/" + action + "/" + qs);
        }
        protected string urlIndex(string action)
        {
            var qs = String.IsNullOrEmpty(queryString) ? "" : ("?" + queryString);
            return ("/" + _controller + "/" + action + "/" + qs);
        }

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string controller
        {
            get { return (_controller); }
            set { _controller = value; }
        }
        public string queryString { get; set; }
        public void addQueryStringParameter(String param) { queryString = String.IsNullOrEmpty(queryString) ? param : queryString + "&" + param; }

        public string details
        {
            get { return (urlConstruct(_controller, _details ?? "details")); }
            set { _details = value; }
        }
        public string edit
        {
            get { return (urlConstruct(_controller, _edit ?? "edit")); }
            set { _edit = value; }
        }
        public string delete
        {
            get { return (urlConstruct(_controller, _delete ?? "delete")); }
            set { _delete = value; }
        }
        
        public string index
        {
            get { return (overrideIndex ?? urlIndex(_index ?? "index")); }
            set { _index = value; }
        }
        public string overrideIndex { get; set; }
        public string create
        {
            get { return (urlCreate(_create ?? "create")); }
            set { _create = value; }
        }

    }
    
}
