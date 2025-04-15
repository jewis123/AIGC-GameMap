using MapGenerator.Wnds;

namespace MapGenerator
{
    public partial class Enter : Form
    {

        private int selectMapxIdx;
        private bool creationListInited = false;

        public Enter()
        {
            InitializeComponent();

            this.tabControl1.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl1.SelectedIndex == 0 && creationListInited == false)
                {
                    InitializeCreationMapListView();

                    creationListInited = true;
                }
                else
                {

                }
            };
        }

        private void InitializeCreationMapListView()
        {
            listView.View = View.LargeIcon;


            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(128, 128);


            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dRPG.png")));
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dTopDown.png")));
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/coming.png")));
            listView.LargeImageList = imageList;

            ListViewItem item1 = new ListViewItem("2dRPG");
            item1.ImageIndex = 0;
            listView.Items.Add(item1);

            ListViewItem item2 = new ListViewItem("2dTopDown");
            item2.ImageIndex = 1;
            listView.Items.Add(item2);

            ListViewItem item3 = new ListViewItem("coming soon");
            item3.ImageIndex = 2;
            listView.Items.Add(item3);


            listView.ItemSelectionChanged += ListView_ItemSelectionChanged;
        }

        private void ListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {
            selectMapxIdx = e.ItemIndex;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            MainEditor win = new MainEditor();
            win.SetMapIndex(selectMapxIdx);
            win.Show();
            this.Hide();
        }

        private void newBtn_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

        }
    }
}
