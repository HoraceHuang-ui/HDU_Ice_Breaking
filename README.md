# 👌HDU_Ice_Breaking 杭电助手破冰

- 杭电圣光机联合学院  计算机科学与技术  21321108  黄一语
## 平台：通用 Windows (UWP)
### ❔ 如何安装
1. 在 **设置** ->**更新与安全** -> **开发者选项**中，开启 **开发人员模式**
2. 在 `Releases` 中获取安装包
3. 解压后将证书 `ice_breaking_cert.pfx` 安装到 **本地计算机**，密码是 ***hyy12345***，存储到 **受信任的根证书颁发机构** 中
4. 打开 `Ice_Breaking_0.9.3.0_x64.msix` 文件，点击安装即可
### ⌨️ 开发说明
- 使用语言：前端 XAML、后端 C#
- IDE: Visual Studio 2019 Community
- NuGet: Microsoft/***Microsoft.UI.Xaml***   ,  aaubry/***YamlDotNet***
### 🐱‍💻 解决方案
- 主页: `MainPage.xaml`
- 类库: `Class1.cs`
- 操作页: `quiz.xaml`
- 数据库: `student_data.txt`
- 类库中封装了两个类，`Person` 和 `DataInitializer`。定义如下：
```cs
    public class Person
    {
        public string name;  //这五个请阅读注意事项中的数据格式解释
        public string id;
        public string male;
        public string anonym;
        public string photo;

        public Person();  //名字、id、照片为空, male T, anonym F
        public Person(string n, string i, string m, string a, string ph);
        public void readFromStr(string s);  //利用 YamlDotNet 将字符串内容提取进类里
    }
```
```cs
    public class DataInitializer
    {
        public List<Person> person = new List<Person> { };  //存储了所有学生信息
        public string raw_str = "";  //从文件中读出的所有学生信息
        public List<string> s_list = new List<string> { };  //分离后的学生信息，传给 Person.readFromStr

        public async Task init_data_async();  //给 person 赋值
        private List<string> findMatch(string a);  //为 init_data_async() 服务，从 raw_str 分离出一堆字符串
    }
```
### ⚠️ 注意事项
> 本人巨菜，代码可读性可能有点低、算法可能很暴力原始，
> 存数据库更是离谱地用 txt 读字符串，请大佬们轻喷🥲
> ——hyy
- 本程序使用 **对分查找** 实现学号校验，请务必保证数据库文档中学号升序排列
- 保存学生数据的文件请放在 `C:\users\<username>\documents\ice_breaking` 中，文件名为 `student_data.txt`。运行一次软件应该会自动在那个路径里生成一个空的。
- `student_data.txt` 中数据格式示例：
```yaml
name: 黄一语
id: 21321108
male: T
anonym: F
photo: https://www.baidu.com/img/PC_7ac6a6d319ba4ae29b38e5e4280e9122.png
#最后加个空行，两条数据之间可加可不加
```
- 数据格式解释：
  - `name` 姓名，由于我用的是正则表达式提取所以务必为中文
  - `id` 学号，务必为升序
  - `male` 参数分为 T 或 F，这样比如若生成照片是男生，那么下面选项中就不会出现女生 **（还没做）**
  - `anonym` 匿名，在主页勾选后即为 T，然后照片和选项中都不会出现 ta
  - `photo` 照片链接，可以是本地路径也可以是网页路径
