using Heiflow.Core.Data.ODM;
using Heiflow.Core.Drawing;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Heiflow.Visualization.Renderable
{
    public class RenderableSitesLayer
    {
        private World _World;
        public event EventHandler<Site> ViewSiteDataClicked;
        public event EventHandler<Site> SitePropertyClicked;

        public RenderableSitesLayer(World world, ClickableIcons icons)
        {
            _World = world;
            SiteIcons = icons; 
            SiteIcons.IconLeftButtonClicked += ObservationSitesLayer_IconLeftButtonClicked;
        }

        public ClickableIcons SiteIcons
        {
            get;
            private set;
        }

        public void ShowSites(Site[] sites)
        {
            if (sites == null)
                return;
            string textureName = ConfigurationManager.Engine3DSettings.IconPath + "1.png";
            SiteIcons.RemoveAll();

            var gg = from ss in sites group ss by ss.SiteType into gp select new { cat = gp.Key };
            var count = gg.Count();
            Dictionary<string, string> txtures = new Dictionary<string, string>();
            for (int i = 1; i <= count; i++)
            {
                if (i <= 10)
                {
                    string fn = string.Format("{0}{1}.png", ConfigurationManager.Engine3DSettings.IconPath, i);
                    txtures.Add(gg.ElementAt(i - 1).cat, fn);
                }
                else
                {
                    string fn = string.Format("{0}{1}.png", ConfigurationManager.Engine3DSettings.IconPath, 1);
                    txtures.Add(gg.ElementAt(i - 1).cat, fn);
                }
            }
            foreach (var site in sites)
            {
                double terrainHeight = 0;
                if (_World.TerrainAccessor != null)
                {
                    terrainHeight = _World.TerrainAccessor.GetElevationAt(site.Longitude, site.Latitude);
                }
                System.Windows.Forms.MenuItem[] mitems = new System.Windows.Forms.MenuItem[2];
                mitems[0] = new System.Windows.Forms.MenuItem("View Data", OnViewDataClicked);
                mitems[1] = new System.Windows.Forms.MenuItem("Property", OnPropertyClicked);
                textureName = txtures[site.SiteType];
                Icon ic = new Icon(site.Name, site.Comments, site.Latitude, site.Longitude, terrainHeight, _World, textureName, 16, 16, "");
                ic.IsOn = true;

                ic.Altitude = terrainHeight;
                ic.isSelectable = true;
                ic.NameAlwaysVisible = true;
                ic.MinimumDisplayDistance = 0;
                ic.MaximumDisplayDistance = 100000000;
                ic.ParentControl = ConfigurationManager.Engine3DSettings.SceneWindow;
                var cm = new System.Windows.Forms.ContextMenu(mitems);
                ic.ContextMenu = cm;
                cm.Tag = site;
                ic.Tag = site;
                foreach (var item in mitems)
                    item.Tag = site;
                SiteIcons.Add(ic);
            }
        }

        private void ObservationSitesLayer_IconLeftButtonClicked(object sender, ClickableIconEventArgs e)
        {
            if (e.Icon.Tag == null  || !SiteIcons.Clickable)
                return;

            if (ViewSiteDataClicked != null)
                ViewSiteDataClicked(this, e.Icon.Tag as Site);
        }

        private void OnViewDataClicked(object sender, EventArgs e)
        {
            if (ViewSiteDataClicked != null)
            {
                var cm = sender as System.Windows.Forms.MenuItem;
                 var site = cm.Tag as Site;
                 ViewSiteDataClicked(this, site);
            }
        }

        private void OnPropertyClicked(object sender, EventArgs e)
        {
            if (SitePropertyClicked != null)
            {
                var cm = sender as System.Windows.Forms.MenuItem;
                var site = cm.Tag as Site;
                SitePropertyClicked(this, site);
            }
        }
    }
}
