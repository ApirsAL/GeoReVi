namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblThinSectionPointCounting")]
    public partial class tblThinSectionPointCounting 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int tspclabmeIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string tspcMethod { get; set; }

        public int tspcNumberCounts { get; set; }

        [Required]
        [StringLength(255)]
        public string tspcUsedMicroscope { get; set; }

        public int? tspcMonoQuartzN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcMonoQuartzRatio { get; set; }

        public int? tspcPolyQuartzN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcPolyQuartzRatio { get; set; }

        public int? tspcKalifeldsparN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcKalifeldsparRatio { get; set; }

        public int? tspcPlagioclaseN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcPlagioclaseRatio { get; set; }

        public int? tspcBiotiteN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcBiotiteRatio { get; set; }

        public int? tspcMuscoviteN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcMuscoviteRatio { get; set; }

        public int? tspcCalciteN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcCalciteRatio { get; set; }

        public int? tspcAmphiboleN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcAmphiboleRatio { get; set; }

        public int? tspcPyroxeneN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcPyroxeneRatio { get; set; }

        public int? tspcOlivineN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcOlivineRatio { get; set; }

        public int? tspcPlutonicClastN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcPlutonicClastRatio { get; set; }

        public int? tspcMetamorphicClastN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcMetamorphicClastRatio { get; set; }

        public int? tspcVolcanicClastN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcVolcanicClastRatio { get; set; }

        public int? tspcSedimentaryClastN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcSedimentaryClastRatio { get; set; }

        public int? tspcHeavyMineralN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcHeavyMineralRatio { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcRatioClasts { get; set; }

        public int? tspcClayCementN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcClayCementRatio { get; set; }

        public int? tspcQuartzCementN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcQuartzCementRatio { get; set; }

        public int? tspcCarbonateCementN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcCarbonateCementRatio { get; set; }

        public int? tspcHematiteCementN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcHematiteCementRatio { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcRatioCements { get; set; }

        public int? tspcMatrixN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcMatrixRatio { get; set; }

        public int? tspcPorosityN { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? tspcPorosityRatio { get; set; }
    }
}
