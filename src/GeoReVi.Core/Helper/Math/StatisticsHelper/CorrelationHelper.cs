using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// A helper class which provides numerical methods for correlation analysis
    /// </summary>
    public class CorrelationHelper : MultiVariateAnalysis
    {

        //The resulting correlation matrix
        private DataTable correlationMatrix;
        public  DataTable CorrelationMatrix
        {
            get => correlationMatrix;
            private set
            {
                this.correlationMatrix = value;
                NotifyOfPropertyChange(() => CorrelationMatrix);
            }
        }

        //The resulting pearson correlation matrix
        private DataTable pearsonCorrelationMatrix;
        public DataTable PearsonCorrelationMatrix
        {
            get => pearsonCorrelationMatrix;
            private set
            {
                this.pearsonCorrelationMatrix = value;
                NotifyOfPropertyChange(() => PearsonCorrelationMatrix);
            }
        }

        //The resulting spearman correlation matrix
        private DataTable spearmanCorrelationMatrix;
        public DataTable SpearmanCorrelationMatrix
        {
            get => spearmanCorrelationMatrix;
            private set
            {
                this.spearmanCorrelationMatrix = value;
                NotifyOfPropertyChange(() => SpearmanCorrelationMatrix);
            }
        }

        /// <summary>
        /// Viewmodel for a pearson correlation chart
        /// </summary>
        private LineAndScatterChartViewModel pearsonBubbleChartViewModel = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PearsonBubbleChartViewModel
        {
            get => pearsonBubbleChartViewModel;
            set
            {
                this.pearsonBubbleChartViewModel = value;
                NotifyOfPropertyChange(() => PearsonBubbleChartViewModel);
            }
        }

        /// <summary>
        /// Viewmodel for a spearman correlation chart
        /// </summary>
        private LineAndScatterChartViewModel spearmanBubbleChartViewModel = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel SpearmanBubbleChartViewModel
        {
            get => spearmanBubbleChartViewModel;
            set
            {
                this.spearmanBubbleChartViewModel = value;
                NotifyOfPropertyChange(() => SpearmanBubbleChartViewModel);
            }
        }

        #region Default constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CorrelationHelper()
        {
        }

        #endregion

        /// <summary>
        /// Calculates a correlation matrix from a numerical data table
        /// </summary>
        /// <returns></returns>
        public async Task CorrelationMatrixCalculation()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };

            await ch.RunBackgroundWorkerWithFlagHelperAsync(()=> IsComputing, async() =>
            {
                int columnCounts = Merge.Columns.Count;

                Array.Resize(ref correlationMatrix, columnCounts);

                for (int i = 0; i < Merge.Columns.Count; i++)
                {
                    try
                    {
                        double[] correlations = new double[] { };
                        Array.Resize(ref correlations, columnCounts);

                        for (int j = 0; j < columnCounts; j++)
                        {
                            try
                            {
                                List<XY> pair = new List<XY>();
                                pair = new List<XY>(Merge.AsEnumerable()
                                        .Select(x => new XY()
                                        {
                                            X = Convert.ToDouble(x.Field<double>(i)),
                                            Y = Convert.ToDouble(x.Field<double>(j))
                                        }));

                                pair = pair.Where(x => !Double.IsNaN(x.X) && x.X != 0
                                             && !Double.IsNaN(x.Y) && x.Y != 0).ToList();

                                double averageI = pair.Average(x => x.X);
                                double averageJ = pair.Average(x => x.Y);

                                correlations[j] = Math.Round((1 / ((double)pair.Count - 1)) * pair.Select(x => (x.X - averageI) * (x.Y - averageJ)).Sum(), 3);

                            }
                            catch
                            {
                                correlations[j] = double.NaN;
                                continue;
                            }
                        }

                        correlationMatrix[i] = correlations;

                    }
                    catch
                    {
                        continue;
                    }
                }

                CorrelationMatrix = correlationMatrix.JaggedArrayToSymmetricDataTable(Merge.Columns.Cast<DataColumn>()
                     .Select(x => x.ColumnName)
                     .ToArray());

            });

        }

        /// <summary>
        /// Creating a bubble chart
        /// </summary>
        /// <param name="par"></param>
        public async Task CreatePearsonMatrixChart()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };


            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                try
                {
                    if (PearsonCorrelationMatrix == null || PearsonCorrelationMatrix.Rows.Count == 0)
                        return;

                    DataTable dat = PearsonCorrelationMatrix.Copy();
                    dat.Columns.RemoveAt(0);

                    PearsonBubbleChartViewModel.Lco.IsColorMap = true;
                    PearsonBubbleChartViewModel.Lco.Direction = DirectionEnum.XY;
                    PearsonBubbleChartViewModel.Lco.DataSet = new BindableCollection<Mesh>(new List<Mesh>() { new Mesh() { Name = "CorrelationTable", Data = dat } }.ToList());
                    PearsonBubbleChartViewModel.Lco.CreateMatrixChart();
                }
                catch
                {
                    return;
                }
            });
        }


        /// <summary>
        /// Creating a bubble chart
        /// </summary>
        /// <param name="par"></param>
        public async Task CreateSpearmanMatrixChart()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                try
                {
                    if (SpearmanCorrelationMatrix == null || SpearmanCorrelationMatrix.Rows.Count == 0)
                        return;

                    DataTable dat = SpearmanCorrelationMatrix.Copy();
                    dat.Columns.RemoveAt(0);

                    SpearmanBubbleChartViewModel.Lco.IsColorMap = true;
                    SpearmanBubbleChartViewModel.Lco.Direction = DirectionEnum.XY;
                    SpearmanBubbleChartViewModel.Lco.DataSet = new BindableCollection<Mesh>(new List<Mesh>() { new Mesh() { Name = "CorrelationTable", Data = dat } }.ToList());
                    SpearmanBubbleChartViewModel.Lco.CreateMatrixChart();
                }
                catch
                {
                    return;
                }
            });
        }

        /// <summary>
        /// Calculates a correlation matrix from a numerical data table
        /// </summary>
        /// <returns></returns>
        public async Task PearsonCorrelationMatrixCalculation()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };

            int columnCounts = Merge.Columns.Count;

            await ch.RunBackgroundWorkerWithFlagHelperAsync(()=> IsComputing, async() =>
            {
                Array.Resize(ref correlationMatrix, columnCounts);

                for (int i = 0; i < Merge.Columns.Count; i++)
                {
                    try
                    {
                        double[] correlations = new double[] { };
                        Array.Resize(ref correlations, columnCounts);

                        for (int j = 0; j < columnCounts; j++)
                        {
                            try
                            {
                                List<XY> pair = new List<XY>();
                                pair = new List<XY>(Merge.AsEnumerable()
                                        .Select(x => new XY()
                                        {
                                            X = Convert.ToDouble(x.Field<double>(i)),
                                            Y = Convert.ToDouble(x.Field<double>(j))
                                        }));

                                pair = pair.Where(x => !Double.IsNaN(x.X) && x.X != 0
                                             && !Double.IsNaN(x.Y) && x.Y != 0).ToList();

                                double averageI = pair.Average(x => x.X);
                                double averageJ = pair.Average(x => x.Y);
                                double sumVariance = pair.Select(x => (x.X - averageI) * (x.Y - averageJ)).Sum();

                                correlations[j] = sumVariance;
                                correlations[j] = Math.Round(correlations[j] / (Math.Sqrt(pair.Select(x => Math.Pow(x.X - averageI, 2)).Sum() * pair.Select(x => Math.Pow(x.Y - averageJ, 2)).Sum())),3);

                            }
                            catch
                            {
                                correlations[j] = double.NaN;
                                continue;
                            }
                        }

                        correlationMatrix[i] = correlations;

                    }
                    catch
                    {
                        continue;
                    }
                }

                PearsonCorrelationMatrix = correlationMatrix.JaggedArrayToSymmetricDataTable(Merge.Columns.Cast<DataColumn>()
                 .Select(x => x.ColumnName)
                 .ToArray());
            });
        }

        /// <summary>
        /// Calculates a correlation matrix from a numerical data table
        /// </summary>
        /// <returns></returns>
        public async Task SpearmanCorrelationMatrixCalculation()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {

                int columnCounts = Merge.Columns.Count;

                Array.Resize(ref correlationMatrix, columnCounts);

                for (int i = 0; i < Merge.Columns.Count; i++)
                {
                    try
                    {
                        double[] correlations = new double[] { };
                        Array.Resize(ref correlations, columnCounts);

                        for (int j = 0; j < columnCounts; j++)
                        {
                            try
                            {
                                List<XY> ranks = new List<XY>();
                                List<XY> pair = new List<XY>();
                                pair = new List<XY>(Merge.AsEnumerable()
                                        .Select(x => new XY()
                                        {
                                            X = Convert.ToDouble(x.Field<double>(i)),
                                            Y = Convert.ToDouble(x.Field<double>(j))
                                        }));

                                pair = pair.Where(x => !Double.IsNaN(x.X) && x.X != 0
                                             && !Double.IsNaN(x.Y) && x.Y != 0).ToList();


                                List<XY> a = new List<XY>(pair.OrderBy(x => x.X).ToList());
                                List<XY> b = new List<XY>(pair.OrderBy(x => x.Y).ToList());

                                for (int k = 0; k < a.Count(); k++)
                                {
                                    ranks.Add(new XY() { X = a.IndexOf(pair[k]), Y = b.IndexOf(pair[k]) });
                                }


                                var f = (6 * ranks.Select(x => x.X - x.Y).Sum());

                                correlations[j] = Math.Round(1 - ((6 * ranks.Select(x => Math.Pow(x.X - x.Y,2)).Sum()) / (ranks.Count() * (Math.Pow(ranks.Count(), 2) - 1))), 3);

                            }
                            catch
                            {
                                correlations[j] = double.NaN;
                                continue;
                            }
                        }

                        correlationMatrix[i] = correlations;

                    }
                    catch
                    {
                        continue;
                    }
                }

                SpearmanCorrelationMatrix = correlationMatrix.JaggedArrayToSymmetricDataTable(Merge.Columns.Cast<DataColumn>()
                         .Select(x => x.ColumnName)
                         .ToArray());
            });
        }

        public override async Task Compute()
        {
            try
            {
                DataTable dat = new DataTable();

                foreach (Mesh dt in DataSet)
                    dat.Merge(dt.Data);

                dat.RemoveNonNumericColumns();
                dat.RemoveNanRowsAndColumns();
                CollectionHelper.ProcessNumericDataTable(dat);
                dat.TreatMissingValues(MissingData);

                Merge = dat;

                await Task.WhenAll(CorrelationMatrixCalculation());
                await Task.WhenAll(PearsonCorrelationMatrixCalculation());
                await Task.WhenAll(CreatePearsonMatrixChart());
                await Task.WhenAll(SpearmanCorrelationMatrixCalculation());
                await Task.WhenAll(CreateSpearmanMatrixChart());

            }
            catch
            {
                return;
            }
        }
    }
}
