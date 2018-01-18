using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;

namespace _3dedit {
    class CMacro {
        internal string Name;
        internal int NStickers; // >=1
        internal int []Stickers;  // start with face
            internal double[] Point;
        internal int LMacro;
        internal long[] Code;
        internal CMacro(string name,int nstk,int[] stks,double[] pt,int lcode,long[] code) {
            Name=name.Replace(' ','_');
            NStickers=nstk;
            Stickers=new int[nstk];
            for(int i=0;i<NStickers;i++) Stickers[i]=stks[i];
            LMacro=lcode;
            Code=code;
            Point=pt;
        }

    }

    class CMacroFile {
        Hashtable m_htbl;
        string Puzz;
        int dim;
        internal string FileName;

        internal CMacroFile(string puzz,int _dim) {
            m_htbl=new Hashtable();
            FileName=null;
            Puzz=puzz;
            dim=_dim;
        }

        internal bool CheckSize(string puzz) {
            return Puzz==puzz;
        }


        internal void AddMacro(CMacro x) {
            if(m_htbl.ContainsKey(x.Name)) m_htbl[x.Name]=x;
            else m_htbl.Add(x.Name,x);
        }
        internal string[] GetNamesList() {
            return (string[])(new ArrayList(m_htbl.Keys).ToArray(typeof(string)));
        }
        internal CMacro GetMacro(string name) { return (CMacro)m_htbl[name]; }

        internal void Delete(string name) {
            m_htbl.Remove(name);
        }
        internal string Rename(string oldname,string newname) {
            newname=newname.Replace(' ','_');
            if(m_htbl.ContainsKey(newname)) return newname;
            CMacro m=(CMacro)m_htbl[oldname];
            m.Name=newname;
            m_htbl.Remove(oldname);
            m_htbl.Add(newname,m);
            return newname;
        }
        internal void SaveAs(string fn) {
            FileName=fn;
            Save();
        }
        internal void Save() {
            try {
                StreamWriter sw=new StreamWriter(FileName);
                sw.NewLine="\r\n";
                sw.WriteLine("MPUlt Macro File");
                sw.WriteLine("{0} {1} {2}",Puzz,dim,m_htbl.Count);
                foreach(CMacro m in m_htbl.Values) {
                    sw.Write("{0} {1} {2}",m.Name,m.NStickers,m.LMacro);
                    for(int j=0;j<m.NStickers;j++) sw.Write(" {0}",m.Stickers[j]);
                    sw.WriteLine();
                    if(dim==4){ 
                        for(int j=0;j<dim;j++) sw.Write("{0} ",m.Point[j].ToString("F6",CultureInfo.InvariantCulture));
                    }
                    sw.WriteLine();
                    for(int j=0;j<m.LMacro;j++) {
                        sw.Write("{0} ",m.Code[j]);
                        if(j%16==15 || j==m.LMacro-1) sw.WriteLine();
                    }
                }
                sw.Close();
            } catch { }
        }
        internal CMacroFile(string fn){
            try {
                StreamReader sr=new StreamReader(fn);
                string ln=sr.ReadLine();
                if(ln!="MPUlt Macro File") goto _1;
                FileName=fn;
                ln=sr.ReadLine();
                string[] ss=ln.Split(' ');
                Puzz=ss[0];
                dim=int.Parse(ss[1]);
                int nm=int.Parse(ss[2]);
                m_htbl=new Hashtable();
                for(int i=0;i<nm;i++) {
                    ln=sr.ReadLine();
                    ss=ln.Split(' ');
                    string name=ss[0];
                    int nstk=int.Parse(ss[1]);
                    int lm=int.Parse(ss[2]);
                    int[] stks=new int[nstk];
                    for(int j=0;j<nstk;j++) stks[j]=int.Parse(ss[j+3]);
                    double[] pt=null;
                    if(dim==4) {
                        ss=sr.ReadLine().Split(' ');
                        pt=new double[dim];
                        for(int j=0;j<dim;j++) pt[j]=double.Parse(ss[j],CultureInfo.InvariantCulture);
                    }
                    long[] code=new long[lm];
                    int p=0;
                    while(p<lm) {
                        ln=sr.ReadLine();
                        ss=ln.Split(' ');
                        for(int j=0;j<ss.Length;j++) {
                            if(p==lm) break;
                            if(ss[j]!="") code[p++]=long.Parse(ss[j]);
                        }
                    }
                    CMacro mm=new CMacro(name,nstk,stks,pt,lm,code);
                    m_htbl.Add(name,mm);
                }
                sr.Close();
                return;
            } catch { }
            _1:
            m_htbl=new Hashtable();
            FileName=null;
            Puzz="3^4";
            dim=4;
        }

    }
}
