using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _3dedit {
    public partial class TextDialog:Form {
        public TextDialog(string title) {
            InitializeComponent();
            Text=title;
        }
        public string Value {
            get { return textBox1.Text; }
            set { textBox1.Text=value; }
        }
    }
}
