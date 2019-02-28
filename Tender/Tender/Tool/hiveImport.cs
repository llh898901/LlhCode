using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tender.Tool
{
    /// <summary>
    /// hive数据库数据导入sql，/001不可见分隔符
    /// </summary>
    public class HiveImport
    {
        public void Import_tenant_extend_data()
        {
            StreamReader sr = new StreamReader("E:\\log\\eoms_20190215\\tenant_extend_data.dat");
            string s = sr.ReadLine();
            List<tenant_extend_data> list = new List<tenant_extend_data>();
            while (s != null)
            {
                //if (i == 3642)//564176
                //{
                //    s = sr.ReadLine();//读取每行
                //}
                s = sr.ReadLine();//读取每行
                if (s != null)
                {

                    var a = s.Split(new[] { '' }, StringSplitOptions.None);

                    list.Add(new tenant_extend_data()
                    {
                        d0 = a[0],
                        id = a[1],
                        tenant_id = a[2],
                        create_by = a[3],
                        create_date = a[4],
                        update_by = a[5],
                        update_date = a[6],
                        b1 = a[7],
                        b2 = a[8],
                        b3 = a[9],
                        b4 = a[10],
                        b5 = a[11],
                        b6 = a[12],
                        b7 = a[13],
                        b8 = a[14],
                        b9 = a[15],
                        b10 = a[16],
                        d1 = a[17],
                        d2 = a[18],
                        d3 = a[19],
                        d4 = a[20],
                        d5 = a[21],
                        dt1 = a[22],
                        dt2 = a[23],
                        dt3 = a[24],
                        dt4 = a[25],
                        dt5 = a[26],
                        n1 = a[27],
                        n2 = a[28],
                        n3 = a[29],
                        n4 = a[30],
                        n5 = a[31],
                        n6 = a[32],
                        n7 = a[33],
                        n8 = a[34],
                        n9 = a[35],
                        n10 = a[36],
                        n11 = a[37],
                        n12 = a[38],
                        n13 = a[39],
                        n14 = a[40],
                        n15 = a[41],
                        n16 = a[42],
                        n17 = a[43],
                        n18 = a[44],
                        n19 = a[45],
                        n20 = a[46],
                        s1 = a[47],
                        s2 = a[48],
                        s3 = a[49],
                        s4 = a[50],
                        s5 = a[51],
                        s6 = a[52],
                        s7 = a[53],
                        s8 = a[54],
                        s9 = a[55],
                        s10 = a[56],
                        s11 = a[57],
                        s12 = a[58],
                        s13 = a[59],
                        s14 = a[60],
                        s15 = a[61],
                        s16 = a[62],
                        s17 = a[63],
                        s18 = a[64],
                        s19 = a[65],
                        s20 = a[66],
                        ms1 = a[67],
                        ms2 = a[68],
                        ms3 = a[69],
                        ms4 = a[70],
                        ms5 = a[71],
                        ms6 = a[72],
                        ms7 = a[73],
                        ms8 = a[74],
                        ms9 = a[75],
                        ms10 = a[76],
                        ms11 = a[77],
                        ms12 = a[78],
                        ms13 = a[79],
                        ms14 = a[80],
                        ms15 = a[81],
                        ms16 = a[82],
                        ms17 = a[83],
                        ms18 = a[84],
                        ms19 = a[85],
                        ms20 = a[86],
                        bs1 = a[87],
                        bs2 = a[88],
                        bs3 = a[89],
                        bs4 = a[90],
                        bs5 = a[91],
                        bs6 = a[92],
                        bs7 = a[93],
                        bs8 = a[94],
                        bs9 = a[95],
                        bs10 = a[96],
                        bs11 = a[97],
                        bs12 = a[98],
                        bs13 = a[99],
                        bs14 = a[100],
                        bs15 = a[101],
                        bs16 = a[102],
                        bs17 = a[103],
                        bs18 = a[104],
                        bs19 = a[105],
                        bs20 = a[106],
                        t1 = a[107],
                        t2 = a[108],
                        t3 = a[109],
                        t4 = a[110],
                        t5 = a[111],
                        t6 = a[112],
                        t7 = a[113],
                        t8 = a[114],
                        t9 = a[115],
                        t10 = a[116]
                    });
                }
                else
                {
                    DosHelper.SqlBuckCopy<tenant_extend_data>(DB.Context, list, "dbo.tenant_extend_data");
                    list.Clear();
                }
                if (list.Count == 10000)
                {
                    DosHelper.SqlBuckCopy<tenant_extend_data>(DB.Context, list, "dbo.tenant_extend_data");
                    //DB.Context.Insert<tenant_extend_data>(list);
                    list.Clear();
                }
            }
        }
    }
}
