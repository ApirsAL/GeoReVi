namespace GeoReVi
{
    using System.Data.Entity;
    using System.Linq;


    public partial class ApirsDatabase : DbContext
    {
        private bool disposed = false;

        public ApirsDatabase(bool connect = true)
            : base("name=ApirsDatabase")
        {
            if (connect)
                try
                {
                    Database.Connection.ConnectionString = CryptographyHelper.Decrypt(tblFrontEndAuthentication.First().faRF, tblFrontEndAuthentication.First().faRT);
                    Database.Connection.Open();
                }
                catch
                {
                }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;
            base.Dispose(disposing);
        }

        public virtual DbSet<tblAffiliation> tblAffiliations { get; set; }
        public virtual DbSet<tblAlia> tblAlias { get; set; }
        public virtual DbSet<tblApparentPermeability> tblApparentPermeabilities { get; set; }
        public virtual DbSet<tblArchEleOccurence> tblArchEleOccurences { get; set; }
        public virtual DbSet<tblArchitecturalElement> tblArchitecturalElements { get; set; }
        public virtual DbSet<tblArchitecturalElementLithostrat> tblArchitecturalElementLithostrats { get; set; }
        public virtual DbSet<tblArchitecturalElementsDepositionalEnvironment> tblArchitecturalElementsDepositionalEnvironments { get; set; }
        public virtual DbSet<tblAxialCompression> tblAxialCompression { get; set; }
        public virtual DbSet<tblAxialCompressionCurve> tblAxialCompressionCurve { get; set; }
        public virtual DbSet<tblBasin> tblBasins { get; set; }
        public virtual DbSet<tblBasinLithoUnit> tblBasinLithoUnits { get; set; }
        public virtual DbSet<tblBiochemicalFacy> tblBiochemicalFacies { get; set; }
        public virtual DbSet<tblBoundingSurfaceArchEle> tblBoundingSurfaceArchEles { get; set; }
        public virtual DbSet<tblBoundingSurface> tblBoundingSurfaces { get; set; }
        public virtual DbSet<tblBoundingSurfaceSubset> tblBoundingSurfaceSubsets { get; set; }
        public virtual DbSet<tblBoreholeTemperature> tblBoreholeTemperature { get; set; }
        public virtual DbSet<tblBulkDensity> tblBulkDensities { get; set; }
        public virtual DbSet<tblCountry> tblCountries { get; set; }
        public virtual DbSet<tblCuboid> tblCuboids { get; set; }
        public virtual DbSet<tblDataQualityRating> tblDataQualityRatings { get; set; }
        public virtual DbSet<tblDepositionalEnvironmentCatalogue> tblDepositionalEnvironmentCatalogues { get; set; }
        public virtual DbSet<tblDepositionalEnvironmentLithostrat> tblDepositionalEnvironmentLithostrats { get; set; }
        public virtual DbSet<tblDrillCore> tblDrillCores { get; set; }
        public virtual DbSet<tblDrilling> tblDrillings { get; set; }
        public virtual DbSet<tblEffectivePorosity> tblEffectivePorosities { get; set; }
        public virtual DbSet<tblEonothem> tblEonothems { get; set; }
        public virtual DbSet<tblErathem> tblErathems { get; set; }
        public virtual DbSet<tblFacy> tblFacies { get; set; }
        public virtual DbSet<tblFaciesCode> tblFaciesCodes { get; set; }
        public virtual DbSet<tblFaciesLithostrat> tblFaciesLithostrats { get; set; }
        public virtual DbSet<tblFaciesObservation> tblFaciesObservations { get; set; }
        public virtual DbSet<tblFileFieldMeasurement> tblFileFieldMeasurements { get; set; }
        public virtual DbSet<tblFileLabMeasurement> tblFileLabMeasurements { get; set; }
        public virtual DbSet<tblFileSection> tblFileSections { get; set; }
        public virtual DbSet<tblFormation> tblFormations { get; set; }
        public virtual DbSet<tblFrontEndAuthentication> tblFrontEndAuthentication { get; set; }
        public virtual DbSet<tblGrainDensity> tblGrainDensities { get; set; }
        public virtual DbSet<tblGrainSize> tblGrainSize { get; set; }
        public virtual DbSet<tblGrainSizeCurve> tblGrainSizeCurve { get; set; }
        public virtual DbSet<tblGroup> tblGroups { get; set; }
        public virtual DbSet<tblHandpiece> tblHandpieces { get; set; }
        public virtual DbSet<tblHydraulicHead> tblHydraulicHeads { get; set; }
        public virtual DbSet<tblIgneousFacy> tblIgneousFacies { get; set; }
        public virtual DbSet<tblIsotopes> tblIsotopes { get; set; }
        public virtual DbSet<tblIntrinsicPermeability> tblIntrinsicPermeabilities { get; set; }
        public virtual DbSet<tblLithofacy> tblLithofacies { get; set; }
        public virtual DbSet<tblLithoStratigraphySection> tblLithoStratigraphySection { get; set; }
        public virtual DbSet<tblLithofaciesArchitecturalElement> tblLithofaciesArchitecturalElements { get; set; }
        public virtual DbSet<tblMeasurement> tblMeasurements { get; set; }
        public virtual DbSet<tblMeasuringDevice> tblMeasuringDevices { get; set; }
        public virtual DbSet<tblMessage> tblMessages { get; set; }
        public virtual DbSet<tblObjectLithostratigraphy> tblObjectLithostratigraphies { get; set; }
        public virtual DbSet<tblObjectOfInvestigation> tblObjectOfInvestigations { get; set; }
        public virtual DbSet<tblOOILithostrat> tblOOILithostrats { get; set; }
        public virtual DbSet<tblOutcrop> tblOutcrops { get; set; }
        public virtual DbSet<tblPeriod> tblPeriods { get; set; }
        public virtual DbSet<tblPerson> tblPersons { get; set; }
        public virtual DbSet<tblPetrography1> tblPetrography1 { get; set; }
        public virtual DbSet<tblPetrography2> tblPetrography2 { get; set; }
        public virtual DbSet<tblPetrography3> tblPetrography3 { get; set; }
        public virtual DbSet<tblPetrography4> tblPetrography4 { get; set; }
        public virtual DbSet<tblPetrography5> tblPetrography5 { get; set; }
        public virtual DbSet<tblPetrography6> tblPetrography6 { get; set; }
        public virtual DbSet<tblPetrography7> tblPetrography7 { get; set; }
        public virtual DbSet<tblPetrography8> tblPetrography8 { get; set; }
        public virtual DbSet<tblPictureAnalyticalDevice> tblPictureAnalyticalDevices { get; set; }
        public virtual DbSet<tblPictureArchitecturalElement> tblPictureArchitecturalElements { get; set; }
        public virtual DbSet<tblPictureBasin> tblPictureBasins { get; set; }
        public virtual DbSet<tblPictureDepositionalEnvironment> tblPictureDepositionalEnvironments { get; set; }
        public virtual DbSet<tblPictureLithofacy> tblPictureLithofacies { get; set; }
        public virtual DbSet<tblPictureObjectOfInvestigation> tblPictureObjectOfInvestigations { get; set; }
        public virtual DbSet<tblPictureRockSample> tblPictureRockSamples { get; set; }
        public virtual DbSet<tblPicturesRockSample> tblPicturesRockSamples { get; set; }
        public virtual DbSet<tblPlug> tblPlugs { get; set; }
        public virtual DbSet<tblPowder> tblPowders { get; set; }
        public virtual DbSet<tblProject> tblProjects { get; set; }
        public virtual DbSet<tblResistivity> tblResistivity { get; set; }
        public virtual DbSet<tblRockQualityDesignationIndex> tblRockQualityDesignationIndex { get; set; }
        public virtual DbSet<tblRockSample> tblRockSamples { get; set; }
        public virtual DbSet<tblSection> tblSections { get; set; }
        public virtual DbSet<tblSectionLithofacy> tblSectionLithofacies { get; set; }
        public virtual DbSet<tblSectionsPetrography> tblSectionsPetrographies { get; set; }
        public virtual DbSet<tblSedimentaryStructure> tblSedimentaryStructures { get; set; }
        public virtual DbSet<tblSery> tblSeries { get; set; }
        public virtual DbSet<tblSonicWave> tblSonicWaves { get; set; }
        public virtual DbSet<tblSonicLog> tblSonicLog { get; set; }
        public virtual DbSet<tblSpectralGammaRay> tblSpectralGammaRays { get; set; }
        public virtual DbSet<tblStage> tblStages { get; set; }
        public virtual DbSet<tblStructureOrientation> tblStructureOrientations { get; set; }
        public virtual DbSet<tblSubformation> tblSubformations { get; set; }
        public virtual DbSet<tblSubgroup> tblSubgroups { get; set; }
        public virtual DbSet<tblSusceptibility> tblSusceptibilities { get; set; }
        public virtual DbSet<tblSystem> tblSystems { get; set; }
        public virtual DbSet<tblThermalConductivity> tblThermalConductivities { get; set; }
        public virtual DbSet<tblThermalDiffusivity> tblThermalDiffusivities { get; set; }
        public virtual DbSet<tblThinSection> tblThinSections { get; set; }
        public virtual DbSet<tblThinSectionPointCounting> tblThinSectionPointCountings { get; set; }
        public virtual DbSet<tblTotalGammaRay> tblTotalGammaRays { get; set; }
        public virtual DbSet<tblTransect> tblTransects { get; set; }
        public virtual DbSet<tblUnionChronostratigraphy> tblUnionChronostratigraphies { get; set; }
        public virtual DbSet<tblUnionLithostratigraphy> tblUnionLithostratigraphies { get; set; }
        public virtual DbSet<tblUnionPetrography> tblUnionPetrographies { get; set; }
        public virtual DbSet<tblVolcanicFacy> tblVolcanicFacies { get; set; }
        public virtual DbSet<tblWater> tblWater { get; set; }
        public virtual DbSet<tblXRayFluorescenceSpectroscopy> tblXRayFluorescenceSpectroscopies { get; set; }
        public virtual DbSet<v_FileStore> v_FileStore { get; set; }
        public virtual DbSet<v_PersonsProject> v_PersonsProject { get; set; }
        public virtual DbSet<v_PetrophysicsRockSamples> v_PetrophysicsRockSamples { get; set; }
        public virtual DbSet<v_PetrophysicsFieldMeasurements> v_PetrophysicsFieldMeasurements { get; set; }
        public virtual DbSet<v_PictureStore> v_PictureStore { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblApparentPermeability>()
                .Property(e => e.apermData)
                .IsUnicode(false);

            modelBuilder.Entity<tblApparentPermeability>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblArchitecturalElement>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblArchitecturalElement>()
                .HasMany(e => e.tblArchEleOccurences)
                .WithRequired(e => e.tblArchitecturalElement)
                .HasForeignKey(e => e.aeIdFk);

            modelBuilder.Entity<tblArchitecturalElement>()
                .HasMany(e => e.tblArchitecturalElementLithostrats)
                .WithRequired(e => e.tblArchitecturalElement)
                .HasForeignKey(e => e.aeIdFk);

            modelBuilder.Entity<tblArchitecturalElement>()
                .HasMany(e => e.tblArchitecturalElementsDepositionalEnvironments)
                .WithRequired(e => e.tblArchitecturalElement)
                .HasForeignKey(e => e.archIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblArchitecturalElement>()
                .HasMany(e => e.tblBoundingSurfaceArchEles)
                .WithRequired(e => e.tblArchitecturalElement)
                .HasForeignKey(e => e.aeIdFk);

            modelBuilder.Entity<tblArchitecturalElement>()
                .HasMany(e => e.tblLithofaciesArchitecturalElements)
                .WithRequired(e => e.tblArchitecturalElement)
                .HasForeignKey(e => e.archIdFk);

            modelBuilder.Entity<tblAxialCompression>()
                .HasOptional(e => e.tblAxialCompressionCurve)
                .WithRequired(e => e.tblAxialCompression)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblBasin>()
                .HasMany(e => e.tblBasinLithoUnits)
                .WithRequired(e => e.tblBasin)
                .HasForeignKey(e => e.basIdFk);

            modelBuilder.Entity<tblBoundingSurface>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblBoundingSurface>()
                .HasMany(e => e.tblArchEleOccurences)
                .WithOptional(e => e.tblBoundingSurface)
                .HasForeignKey(e => e.aoBoundingSurfaceIdTop);

            modelBuilder.Entity<tblBoundingSurface>()
                .HasMany(e => e.tblArchEleOccurences1)
                .WithOptional(e => e.tblBoundingSurface1)
                .HasForeignKey(e => e.aoBoundingSurfaceIdBottom);

            modelBuilder.Entity<tblBoundingSurface>()
                .HasMany(e => e.tblBoundingSurfaceArchEles)
                .WithRequired(e => e.tblBoundingSurface)
                .HasForeignKey(e => e.bsidFk);

            modelBuilder.Entity<tblBulkDensity>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblCountry>()
                .Property(e => e.NAME)
                .IsUnicode(false);

            modelBuilder.Entity<tblCountry>()
                .Property(e => e.CAPITAL)
                .IsUnicode(false);

            modelBuilder.Entity<tblCuboid>()
                .Property(e => e.cubOrientationToBedding)
                .IsUnicode(false);

            modelBuilder.Entity<tblCuboid>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDepositionalEnvironmentCatalogue>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblDepositionalEnvironmentCatalogue>()
                .HasMany(e => e.tblArchitecturalElementsDepositionalEnvironments)
                .WithRequired(e => e.tblDepositionalEnvironmentCatalogue)
                .HasForeignKey(e => e.depenvIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblDepositionalEnvironmentCatalogue>()
                .HasMany(e => e.tblDepositionalEnvironmentLithostrats)
                .WithRequired(e => e.tblDepositionalEnvironmentCatalogue)
                .HasForeignKey(e => e.depenvIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblDepositionalEnvironmentCatalogue>()
                .HasMany(e => e.tblPictureDepositionalEnvironments)
                .WithRequired(e => e.tblDepositionalEnvironmentCatalogue)
                .HasForeignKey(e => e.picIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblDrilling>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblEffectivePorosity>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblEonothem>()
                .HasMany(e => e.tblErathems)
                .WithOptional(e => e.tblEonothem)
                .HasForeignKey(e => e.eraeonIdFk);

            modelBuilder.Entity<tblErathem>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblErathem>()
                .HasMany(e => e.tblPeriods)
                .WithOptional(e => e.tblErathem)
                .HasForeignKey(e => e.pereraIdFk);

            modelBuilder.Entity<tblFacy>()
                .HasOptional(e => e.tblBiochemicalFacy)
                .WithRequired(e => e.tblFacy)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblFacy>()
                .HasMany(e => e.tblFaciesLithostrats)
                .WithRequired(e => e.tblFacy)
                .HasForeignKey(e => e.facIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblFacy>()
                .HasMany(e => e.tblFaciesObservations)
                .WithRequired(e => e.tblFacy)
                .HasForeignKey(e => e.fofacIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblFacy>()
                .HasOptional(e => e.tblIgneousFacy)
                .WithRequired(e => e.tblFacy)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblFacy>()
                .HasOptional(e => e.tblVolcanicFacy)
                .WithRequired(e => e.tblFacy)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblFacy>()
                .HasOptional(e => e.tblLithofacy)
                .WithRequired(e => e.tblFacy)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblFacy>()
                .HasMany(e => e.tblLithofaciesArchitecturalElements)
                .WithRequired(e => e.tblFacy)
                .HasForeignKey(e => e.lftIdFk);

            modelBuilder.Entity<tblFacy>()
                .HasMany(e => e.tblRockSamples)
                .WithOptional(e => e.tblFacy)
                .HasForeignKey(e => e.sampLithofacies);

            modelBuilder.Entity<tblFacy>()
                .HasMany(e => e.tblSectionLithofacies)
                .WithRequired(e => e.tblFacy)
                .HasForeignKey(e => e.litsecIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblFormation>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblFormation>()
                .HasMany(e => e.tblSubformations)
                .WithOptional(e => e.tblFormation)
                .HasForeignKey(e => e.sffmId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblGrainDensity>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblGroup>()
                .HasMany(e => e.tblSubgroups)
                .WithOptional(e => e.tblGroup)
                .HasForeignKey(e => e.sggrIdFk)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblIntrinsicPermeability>()
                .Property(e => e.inpeLength)
                .HasPrecision(20, 2);

            modelBuilder.Entity<tblIntrinsicPermeability>()
                .Property(e => e.inpeRadius)
                .HasPrecision(20, 2);

            modelBuilder.Entity<tblIntrinsicPermeability>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblMeasurement>()
    .Property(e => e.measDate)
    .HasPrecision(0);
            modelBuilder.Entity<tblMeasurement>()
    .HasOptional(e => e.tblAxialCompression)
    .WithRequired(e => e.tblMeasurement)
    .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblApparentPermeability)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblIsotopes)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblBulkDensity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblEffectivePorosity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblGrainDensity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
    .HasOptional(e => e.tblGrainSize)
    .WithRequired(e => e.tblMeasurement)
    .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblIntrinsicPermeability)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblResistivity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblSonicWave)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblThermalConductivity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblThermalDiffusivity)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblXRayFluorescenceSpectroscopy)
                .WithRequired(e => e.tblMeasurement);

            modelBuilder.Entity<tblMeasurement>()
               .HasOptional(e => e.tblStructureOrientation)
               .WithRequired(e => e.tblMeasurement)
               .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
    .HasOptional(e => e.tblSonicLog)
    .WithRequired(e => e.tblMeasurement)
    .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblSpectralGammaRay)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
    .HasOptional(e => e.tblRockQualityDesignationIndex)
    .WithRequired(e => e.tblMeasurement)
    .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
.HasOptional(e => e.tblBoreholeTemperature)
.WithRequired(e => e.tblMeasurement)
.WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblSusceptibility)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
                .HasOptional(e => e.tblTotalGammaRay)
                .WithRequired(e => e.tblMeasurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblMeasurement>()
    .HasOptional(e => e.tblHydraulicHead)
    .WithRequired(e => e.tblMeasurement)
    .WillCascadeOnDelete();

            modelBuilder.Entity<tblObjectOfInvestigation>()
                .HasMany(e => e.tblArchEleOccurences)
                .WithRequired(e => e.tblObjectOfInvestigation)
                .HasForeignKey(e => e.aoOoiIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblOutcrop>()
                .Property(e => e.outLastJourney)
                .HasPrecision(0);

            modelBuilder.Entity<tblOutcrop>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPeriod>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPeriod>()
                .HasMany(e => e.tblSeries)
                .WithOptional(e => e.tblPeriod)
                .HasForeignKey(e => e.serperIdFk);

            modelBuilder.Entity<tblPeriod>()
                .HasMany(e => e.tblSystems)
                .WithOptional(e => e.tblPeriod)
                .HasForeignKey(e => e.sysperIdFk);

            modelBuilder.Entity<tblPerson>()
                .HasMany(e => e.tblFaciesObservations)
                .WithOptional(e => e.tblPerson)
                .HasForeignKey(e => e.foPersonIdFk);

            modelBuilder.Entity<tblPerson>()
                .HasMany(e => e.tblMessages)
                .WithRequired(e => e.tblPerson)
                .HasForeignKey(e => e.messFromPersonIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblPerson>()
                .HasMany(e => e.tblMessages1)
                .WithRequired(e => e.tblPerson1)
                .HasForeignKey(e => e.messToPersonIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblPerson>()
                .HasMany(e => e.tblProjects)
                .WithRequired(e => e.tblPerson)
                .HasForeignKey(e => e.prjCreatorIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblPetrography1>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPetrography1>()
                .HasMany(e => e.tblPetrography2)
                .WithRequired(e => e.tblPetrography1)
                .HasForeignKey(e => e.petr2petr1IdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblPetrography2>()
                .HasMany(e => e.tblPetrography3)
                .WithOptional(e => e.tblPetrography2)
                .HasForeignKey(e => e.petr3petr2IdFk);

            modelBuilder.Entity<tblPetrography3>()
                .HasMany(e => e.tblPetrography4)
                .WithOptional(e => e.tblPetrography3)
                .HasForeignKey(e => e.petr4petr3IdFk);

            modelBuilder.Entity<tblPetrography4>()
                .HasMany(e => e.tblPetrography5)
                .WithOptional(e => e.tblPetrography4)
                .HasForeignKey(e => e.petr5petr4IdFk);

            modelBuilder.Entity<tblPetrography5>()
                .HasMany(e => e.tblPetrography6)
                .WithOptional(e => e.tblPetrography5)
                .HasForeignKey(e => e.petr6petr5IdFk);

            modelBuilder.Entity<tblPetrography6>()
                .HasMany(e => e.tblPetrography7)
                .WithOptional(e => e.tblPetrography6)
                .HasForeignKey(e => e.petr7petr6IdFk);

            modelBuilder.Entity<tblPetrography7>()
                .HasMany(e => e.tblPetrography8)
                .WithOptional(e => e.tblPetrography7)
                .HasForeignKey(e => e.petr8petr7IdFk);

            modelBuilder.Entity<tblPlug>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblPowder>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblProject>()
                .HasMany(e => e.tblRockSamples)
                .WithOptional(e => e.tblProject)
                .HasForeignKey(e => e.sampprjIdFk);

            modelBuilder.Entity<tblResistivity>();

            modelBuilder.Entity<tblRockSample>()
                .Property(e => e.sampDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblRockSample>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblRockSample>()
                .HasMany(e => e.tblPicturesRockSamples)
                .WithRequired(e => e.tblRockSample)
                .HasForeignKey(e => e.sampIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblSection>()
                .HasMany(e => e.tblFileSections)
                .WithRequired(e => e.tblSection)
                .HasForeignKey(e => e.secIdFk);

            modelBuilder.Entity<tblSection>()
                .HasMany(e => e.tblSectionLithofacies)
                .WithRequired(e => e.tblSection)
                .HasForeignKey(e => e.litsecIdFk);

            modelBuilder.Entity<tblSection>()
                .HasMany(e => e.tblLithoStratigraphySection)
                .WithRequired(e => e.tblSection)
                .HasForeignKey(e => e.lithosecIdFk);

            modelBuilder.Entity<tblLithoStratigraphySection>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSectionLithofacy>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSectionsPetrography>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSery>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSery>()
                .HasMany(e => e.tblStages)
                .WithOptional(e => e.tblSery)
                .HasForeignKey(e => e.stageserIdFk);

            modelBuilder.Entity<tblSonicWave>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSpectralGammaRay>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblStage>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblStructureOrientation>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSubgroup>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSubgroup>()
                .HasMany(e => e.tblFormations)
                .WithOptional(e => e.tblSubgroup)
                .HasForeignKey(e => e.fmsgIdFk)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblSusceptibility>()
                .Property(e => e.susDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblSusceptibility>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSystem>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblSystem>()
                .HasMany(e => e.tblSeries)
                .WithOptional(e => e.tblSystem)
                .HasForeignKey(e => e.sersysIdFk);

            modelBuilder.Entity<tblThermalConductivity>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblThermalDiffusivity>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblThinSection>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblTotalGammaRay>()
                .Property(e => e.tgrDate)
                .HasPrecision(0);

            modelBuilder.Entity<tblTotalGammaRay>()
                .Property(e => e.SSMA_TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblErathems)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.eraName);

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblPeriods)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.perName);

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblRockSamples)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.sampChronStratName);

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblSeries)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.serName);

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblStages)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.stageName);

            modelBuilder.Entity<tblUnionChronostratigraphy>()
                .HasMany(e => e.tblSystems)
                .WithOptional(e => e.tblUnionChronostratigraphy)
                .HasForeignKey(e => e.sysName);

            modelBuilder.Entity<tblUnionLithostratigraphy>()
                .HasMany(e => e.tblArchitecturalElementLithostrats)
                .WithRequired(e => e.tblUnionLithostratigraphy)
                .HasForeignKey(e => e.litIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblUnionLithostratigraphy>()
                .HasMany(e => e.tblBasinLithoUnits)
                .WithRequired(e => e.tblUnionLithostratigraphy)
                .HasForeignKey(e => e.lithID);

            modelBuilder.Entity<tblUnionLithostratigraphy>()
                .HasMany(e => e.tblDepositionalEnvironmentLithostrats)
                .WithRequired(e => e.tblUnionLithostratigraphy)
                .HasForeignKey(e => e.litIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblUnionLithostratigraphy>()
                .HasMany(e => e.tblFaciesLithostrats)
                .WithRequired(e => e.tblUnionLithostratigraphy)
                .HasForeignKey(e => e.litIdFk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblFacies)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.facPetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography1)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr1PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography2)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr2PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography3)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr3PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography4)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr4PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography5)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr5PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography6)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr6PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography7)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr7PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblPetrography8)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petr8PetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblRockSamples)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.sampPetrographicTerm);

            modelBuilder.Entity<tblUnionPetrography>()
                .HasMany(e => e.tblSectionsPetrographies)
                .WithOptional(e => e.tblUnionPetrography)
                .HasForeignKey(e => e.petsecPetrgraphy);
        }
    }
}
