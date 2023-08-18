using UnityEngine;
using System.Data;
using System.IO;
using Excel;
using UnityEditor;

#if UNITY_EDITOR 
public class ExcelTool : MonoBehaviour
{
    public class Excel_Tool
    {
        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="columnNum">行数</param>
        /// <param name="rowNum">列数</param>
        /// <param name="excelIndex">表单索引</param>
        /// <returns></returns>
        static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum, int excelIndex = 0)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //Tables[0] 下标0表示excel文件中第一张表的数据
            columnNum = result.Tables[excelIndex].Columns.Count;
            rowNum = result.Tables[excelIndex].Rows.Count;
            return result.Tables[excelIndex].Rows;
        }
        /// <summary>
        /// 读取表数据，生成对应的数组
        /// </summary>
        /// <param name="filePath">excel文件全路径</param>
        /// <returns>Item数组</returns>

        public static BuilderExcelData[] CreatePlayerDefaultDataWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum, 0);

            //根据excel的定义，第二行开始才是数据
            BuilderExcelData[] array = new BuilderExcelData[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                BuilderExcelData item = new BuilderExcelData();
                //解析每列的数据
                item.id = collect[i][0].ToString();
                item.dataName = collect[i][1].ToString();
                item.loadPath = collect[i][2].ToString();
                Debug.Log(collect[i][3].ToString());
                item.selectNodeX = int.Parse(collect[i][3].ToString().Split('_')[0]);
                item.selectNodeY = int.Parse(collect[i][3].ToString().Split('_')[1]);
                item.selectNodeX_90 = int.Parse(collect[i][4].ToString().Split('_')[0]);
                item.selectNodeY_90 = int.Parse(collect[i][4].ToString().Split('_')[1]);
                item.buildTag = collect[i][5].ToString();

                array[i - 1] = item;
            }
            return array;

        }
       
    }
    public class ExcelBuild : Editor
    {
        [MenuItem("CustomEditor/CreateItemAsset")]
        public static void CreateItemAsset()
        {
            BuilderExcelDataLst builderExcelData = CreateInstance<BuilderExcelDataLst>();
            //赋值
            builderExcelData.datas = Excel_Tool.CreatePlayerDefaultDataWithExcel(ExcelConfig.excelsFolderPath + "BuildData.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string playerDefaultAssetPath = ExcelConfig.buildExcelDataAssetPath;
            //生成一个Asset文件
            AssetDatabase.CreateAsset(builderExcelData, playerDefaultAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("生成Asset成功！");
        }
    }
}

#endif
public class ExcelConfig
{
    /// <summary>
    /// 存放excel表文件夹的的路径，本例xecel表放在了"Assets/Excels/"当中
    /// </summary>
    public static readonly string excelsFolderPath = Application.dataPath + @"/_GridPlace/" + "/_Data/" + "/Excels/";
    public static readonly string dataFilePath = Application.dataPath + @"/Resources" + @"/_GridPlace/DataAssets";

    /// <summary>
    /// 存放Excel转化CS文件的文件夹路径
    /// </summary>
    public static readonly string assetPath = "Assets/Resources/_GridPlace/DataAssets/";
    public static readonly string buildExcelDataAssetPath = string.Format("{0}{1}.asset", assetPath, "BuilderExcelDataLst");


    public static readonly string resourcesBuildExcelDataAssetPath = @"_GridPlace/DataAssets/BuilderExcelDataLst";
}
