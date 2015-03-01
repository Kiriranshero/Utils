using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace System
{
    public static class JsonConfig
    {
        const string SConfig = "ConfigJson.cfg";
        const string SSelect = "_Selected";

        private static JObject ListControl = initJSon();
        private static JObject initJSon()
        {
            return File.Exists(SConfig) ? JObject.Parse(File.ReadAllText(SConfig)) : new JObject();
        }

        private static Dictionary<Type, MethodInfo> CtrlSwitch = initSwitch();
        private static Dictionary<Type, MethodInfo> initSwitch()
        {
            var Result = new Dictionary<Type, MethodInfo>();

            foreach (var Method in typeof(JsonConfig).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                var Params = Method.GetParameters();
                if (Params.Count() != 1) continue;
                var MethodParam = Params[0].ParameterType;                
                if (MethodParam.IsSubclassOf(typeof(Control)) == false) continue;
                Result.Add(MethodParam, Method);
            }
            
            return Result;
        }

        public static void AddControl(Control ctrl)
        {
            var ctrlType = ctrl.GetType();
            if (CtrlSwitch.ContainsKey(ctrlType) == false) return;
            CtrlSwitch[ctrlType].Invoke(null, new object[] { ctrl });
        }

        static void SwitchCtrl(TextBoxBase ctrl)
        {
            ctrl.TextChanged += JsonConfig_TextChanged;
            ctrl.TextChanged += FileUpdate;
            if (ListControl[ctrl.Name] == null) return;
            ctrl.Text = ListControl[ctrl.Name].Value<string>();
        }

        static void SwitchCtrl(NumericUpDown ctrl)
        {
            ctrl.ValueChanged += JsonConfig_ValueChanged;
            ctrl.ValueChanged += FileUpdate;
            if (ListControl[ctrl.Name] == null) return;
            ctrl.Value = ListControl[ctrl.Name].Value<decimal>();
            ctrl.Validate();
        }

        static void SwitchCtrl(ComboBox ctrl)
        {
            ctrl.Validated += JsonConfig_Validated;
            ctrl.Validated += FileUpdate;
            ctrl.SelectionChangeCommitted += JsonConfig_SelectionChangeCommitted;
            ctrl.SelectionChangeCommitted += FileUpdate;
            
            if(ListControl[ctrl.Name] != null)
                foreach (var item in ListControl[ctrl.Name].Values<string>())
                    if (ctrl.Items.Contains(item) == false) ctrl.Items.Add(item);
        
            if( ListControl[ctrl.Name + SSelect] != null)
                ctrl.SelectedIndex   = ListControl[ctrl.Name + SSelect].Value<int>();
        }

        static void JsonConfig_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var ctrl = sender as ComboBox;
            ListControl[ctrl.Name + SSelect] = ctrl.SelectedIndex;
        }


        static void JsonConfig_CheckedChanged(object sender, EventArgs e)
        {
            var ctrl = sender as CheckBox;
            ListControl[ctrl.Name] = ctrl.Checked;
        }

        static void JsonConfig_Validated(object sender, EventArgs e)
        {
            var ctrl = sender as ComboBox;
            if (string.IsNullOrEmpty(ctrl.Text)) return;
            if (ctrl.Items.Contains(ctrl.Text)) return;
            ctrl.Items.Add(ctrl.Text);
            ListControl[ctrl.Name] =  new JArray(ctrl.Items);
        }

        static void JsonConfig_TextChanged(object sender, EventArgs e)
        {
            var ctrl = sender as TextBoxBase;
            ListControl[ctrl.Name] = ctrl.Text;
        }

        static void JsonConfig_ValueChanged(object sender, EventArgs e)
        {
            var ctrl = sender as NumericUpDown;
            ListControl[ctrl.Name] = ctrl.Value;
        }

        static void FileUpdate(object sender, EventArgs e)
        {
            File.WriteAllText(SConfig, ListControl.ToString());
        }
    }
}
