[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.3541136.svg)](https://doi.org/10.5281/zenodo.3541136)

# GeoReVi

<img align="right" src="docs/Icon.png" width="50px">
GeoReVi (Geological Reservoir Virtualization) is an open-source data management and analysis tool for subsurface applications. The software is intended to be used for reservoir data collection, data processing, data curation, data visualization and eventually for reservoir characterization. Data is stored in a local or server database dependent on the specific needs. The software runs on all Windows systems.

## License

This project is licensed under the GNU General Public License v3.0 License - see the [LICENSE.md](LICENSE.md) file for details

The *local* version of the software is freely available.

## Requirements
OS: Microsoft Windows

## Built With
* [Windows Presentation Foundation](https://docs.microsoft.com/de-de/dotnet/framework/wpf/) - Main application framework
* [Entity framework](https://docs.microsoft.com/de-de/ef/) - The database framework used
* [HelixToolkit](https://github.com/helix-toolkit/helix-toolkit}{HelixToolkit.WPF) - 3D visualization
* [Managed Extensibility Framework](https://docs.microsoft.com/de-de/dotnet/framework/mef/) - Modular development
* [Accord.NET](http://accord-framework.net) - Linear Algebra and Machine Learning helper
* [Caliburn.Micro](https://caliburnmicro.com/) - Awesome MVVM development framework
* [FontAwesomeWPF](https://github.com/charri/Font-Awesome-WPF/) - Icon provider
* [LiteDB](https://www.litedb.org/) - Embedded NoSQL database
* [GeoAPI](https://github.com/NetTopologySuite/GeoAPI) - Coordinate conversion and projection framework
* [MoreLinq](https://morelinq.github.io) - Helpful Query Framework
* [Extended WPF Toolkit™](https://github.com/xceedsoftware/wpftoolkit) - Providing nice UI controls like the AvalonDock
* [Math.NET](https://www.mathdotnet.com/) - Math magic
* [Alglib](https://www.alglib.net/) - Solving sparse systems of linear equations


## Getting started
To get started with GeoReVi you have to download the packed file [GeoReVi.zip](https://github.com/ApirsAL/GeoReVi/blob/master/binaries/GeoReVi.zip) in the **binaries** folder, unpack it and run the installation file (GeoReVi.exe). Tutorials can be found in the user manual. Exemplary datasets and samples are provided in the **docs** folders and in the zip archive. To setup a multi-user-environment you have to change the section including the connection string in your GeoReVi.exe.config file in the installation folder to the according database. However, then a properly structured database with many stored procedures is needed. Contact us under contact@georevi.com for more information.

### Three-dimensional outcrop model
<img src="docs/Samples/Meshes/OutcropModeling.png" width="50%">

## Contributing
Individual view-models and views can be developed and integrated into GeoReVi.

If bugs or any other issues are identified we would be grateful for reporting those in the [Issues](https://github.com/ApirsAL/GeoReVi/issues) or via [email](contact@georevi.com).

## Authors

* **Adrian Linsel** - *Initial work* - [ApirsAL](https://github.com/ApirsAL) - contact@georevi.com

## Acknowledgments

* Thanks go to all the people that contributed to the test data sets of GeoReVi!

## Samples

### Reservoir scale temperature distribution
<img src="docs/Samples/FrontCover/GitHubFrontCover.png" width="50%">

### Meter-scale permeability distribution
<img src="docs/Samples/Interpolations/RockSlabInterpolation.png" width="35%">

### Three-dimensional principal component analysis
<img src="docs/Samples/MultivariateAnalyses/3D_Principal_Component_Analysis.png" width="50%">

