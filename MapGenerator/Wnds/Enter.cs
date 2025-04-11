using MapGenerator.Wnds;

namespace MapGenerator
{
    public partial class Enter : Form
    {

        private int selectMapxIdx;

        public Enter()
        {
            InitializeComponent();
            // ��ʼ��ListView
            InitializeListView();
        }

        private void InitializeListView()
        {
            // ����ListView����ͼģʽ
            listView.View = View.LargeIcon; // ����ʹ�� SmallIcon��Details ��

            // ����ͼƬ��SmallImageList��LargeImageList
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(128, 128); // ����ͼƬ��С


            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dRPG.png"))); 
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dTopDown.png")));
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/coming.png")));
            listView.LargeImageList = imageList; // ������ListView��LargeImageList

            // ����ListViewItem������ͼƬ����

            ListViewItem item1 = new ListViewItem("2dRPG");
            item1.ImageIndex = 0; // ͼƬ����Ϊ0
            listView.Items.Add(item1);

            ListViewItem item2 = new ListViewItem("2dTopDown");
            item2.ImageIndex = 1; // ͼƬ����Ϊ1
            listView.Items.Add(item2);

            ListViewItem item3 = new ListViewItem("coming soon");
            item3.ImageIndex =2; // ͼƬ����Ϊ0
            listView.Items.Add(item3);


            // ע��ItemSelectionChanged�¼�
            listView.ItemSelectionChanged += ListView_ItemSelectionChanged;
        }

        private void ListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {

            selectMapxIdx = e.ItemIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainEditor win = new MainEditor();
            win.SetMapIndex(selectMapxIdx);
            win.Show();
            this.Hide();
        }
    }
}
