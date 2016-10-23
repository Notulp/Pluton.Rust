namespace Pluton.Rust.Objects {
	using System.Collections.Generic;

	public class PlutonUIEntity {
		public List<Network.Connection> connections;
		public JSON.Array panels = new JSON.Array();

		public PlutonUIEntity(IEnumerable<Network.Connection> cons = null) {
			if (cons != null)
				connections = cons as List<Network.Connection>;
		}

		public PlutonUIEntity(Network.Connection con = null) {
			if (con != null)
				connections = new List<Network.Connection>() { con };
		}

		public PlutonUIPanel AddPanel(string name = null, string parent = null, float? fadeout = null) {
			PlutonUIPanel panel = new PlutonUIPanel(name, parent, fadeout);
			panels.Add(panel.obj);

			return panel;
		}

		public JSON.Array CreateUI() {
			if (connections.Count == 0)
				return null;
            
			CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo() { connections = connections },
			                                                    null,
			                                                    "AddUI",
			                                                    new Facepunch.ObjectList?(new Facepunch.ObjectList(panels.ToString())));

			return panels;
		}

		public void DestroyUI() {
			if (connections.Count == 0)
				return;

			foreach (JSON.Value panel in panels) {
				CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo() { connections = connections },
				                                                       null,
				                                                       "DestroyPanel",
				                                                       new Facepunch.ObjectList?(new Facepunch.ObjectList(panel.Obj.GetString("name"))));
			}
		}

		public PlutonUIPanel GetPanel(string name) {
			for (int i = 0; i < panels.Length; i++) {
				if (panels[i].Obj.GetString(name) == name) {
					return new PlutonUIPanel() { obj = panels[i].Obj };
				}
			}

			return null;
		}

		public bool RemovePanel(string name) {
			for (int i = 0; i < panels.Length; i++) {
				if (panels[i].Obj.GetString(name) == name) {
					panels.Remove(i);

					return true;
				}
			}

			return false;
		}
	}
}
