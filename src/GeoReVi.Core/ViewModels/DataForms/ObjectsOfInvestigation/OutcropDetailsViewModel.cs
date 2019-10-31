
using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GeoReVi
{
    public class OutcropDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblOutcrop selectedOutcrop;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblOutcrop> outcrops;

        //EventAggregator
        public readonly IEventAggregator _events;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblOutcrop SelectedOutcrop
        {
            get { return this.selectedOutcrop; }
            set { this.selectedOutcrop = value; NotifyOfPropertyChange(() => SelectedOutcrop); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblOutcrop> Outcrops
        {
            get { return this.outcrops; }
            set { this.outcrops = value; NotifyOfPropertyChange(() => Outcrops); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public OutcropDetailsViewModel()
        {
            this._events = ((ShellViewModel)IoC.Get<IShell>())._events;
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public OutcropDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblOutcrop>())
            {
                try
                {
                    Outcrops = new BindableCollection<tblOutcrop>(db.GetModelByExpression(outc => outc.outLocalName == name));
                    if (Outcrops.Count == 0)
                    {
                        SelectedOutcrop = new tblOutcrop();
                        SelectedOutcrop.outLocalName = name;
                    }
                    else if (Outcrops.Count > 1)
                    {
                        SelectedOutcrop = Outcrops.First();
                    }
                    else
                    { SelectedOutcrop = Outcrops.First(); }
                }
                catch
                {
                    Outcrops = new BindableCollection<tblOutcrop>();
                    SelectedOutcrop = new tblOutcrop();
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblOutcrop>())
            {
                try
                {
                    if (SelectedOutcrop.outIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedOutcrop);
                            db.Save();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblOutcrop result = db.GetModelById(SelectedOutcrop.outIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedOutcrop, SelectedOutcrop.outIdPk);
                        }
                    }

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
                }
                finally
                {
                }

            }
        }

        public void ShowInfo(string info)
        {
            switch(info)
            {
                case "OutcropArea":
                    break;
            }
        }
    }

    #endregion
}
