namespace Pluton.Rust.Events {
	using Core;
	using Rust;
	using Rust.Objects;

	public class BuildingPartGradeChangeEvent: CountedInstance {
		public readonly BuildingPart BuildingPart;
		public readonly Player Builder;

		public bool HasPrivilege;
		public bool Rotatable = true;
		public bool PayForUpgrade = true;
		public string DestroyReason = string.Empty;
		public bool DoDestroy = false;

		private BuildingGrade.Enum grade;

		public BuildingPartGradeChangeEvent(BuildingBlock buildingBlock,
		                                          BuildingGrade.Enum buildingGrade,
		                                          BasePlayer basePlayer) {
			BuildingPart = new BuildingPart(buildingBlock);
			Builder = Server.GetPlayer(basePlayer);
			grade = buildingGrade;

			HasPrivilege = (bool)buildingBlock.CallMethod("CanChangeToGrade", buildingGrade, basePlayer);
		}

		public BuildingGrade.Enum Grade {
			get { return grade; }
			set { grade = value; }
		}

		public int GradeInt {
			get { return (int)grade; }
			set { grade = (BuildingGrade.Enum)value; }
		}

		public void Destroy(string reason = "Plugin blocks building!") {
			DoDestroy = true;
			DestroyReason = reason;
		}
	}
}
