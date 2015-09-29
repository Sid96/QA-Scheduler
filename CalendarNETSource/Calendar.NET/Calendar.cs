using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System.Threading;

namespace Calendar.NET
{
    /// <summary>
    /// An enumeration describing various ways to view the calendar
    /// </summary>
    public enum CalendarViews
    {
        /// <summary>
        /// Renders the Calendar in a month view
        /// </summary>
        Month = 1,
        /// <summary>
        /// Renders the Calendar in a day view
        /// </summary>
        Day = 2
    }

    /// <summary>
    /// A Winforms Calendar Control
    /// </summary>
    public class Calendar : UserControl
    {
        private static bool commit = false;
        public CustomEvent invisEvent = new CustomEvent();
        public CustomEvent drawNewEvent = new CustomEvent();
        
        public CustomEvent drawOldRelEvent = new CustomEvent();
        public CustomEvent drawRelEvent = new CustomEvent();
 
        public CustomEvent drawOldCust1Event = new CustomEvent();
        public CustomEvent drawCust1Event = new CustomEvent();
        public CustomEvent drawOldCust2Event = new CustomEvent();
        public CustomEvent drawCust2Event = new CustomEvent();
        public CustomEvent drawOldCust3Event = new CustomEvent();
        public CustomEvent drawCust3Event = new CustomEvent();
        public CustomEvent drawOldCust4Event = new CustomEvent();
        public CustomEvent drawCust4Event = new CustomEvent();
        public CustomEvent drawOldCust5Event = new CustomEvent();
        public CustomEvent drawCust5Event = new CustomEvent();
        public CustomEvent drawOldCust6Event = new CustomEvent();
        public CustomEvent drawCust6Event = new CustomEvent();

        public IEvent originalCust1Event = new CustomEvent();
        public IEvent originalCust2Event = new CustomEvent();
        public IEvent originalCust3Event = new CustomEvent();
        public IEvent originalCust4Event = new CustomEvent();
        public IEvent originalCust5Event = new CustomEvent();
        public IEvent originalCust6Event = new CustomEvent();
        public IEvent originalRel = new CustomEvent();
        public Color storeColor = new Color();
        private int numWeeks;
        private DayOfWeek startWeekEnum;
        public static string[] individualData = new string[16];
        public int daysLost;
        public int currentDaysLost;
        public bool move = false;
        private DateTime _calendarDate;
        private Font _dayOfWeekFont;
        private Font _daysFont;
        private Font _todayFont;
        private Font _dateHeaderFont;
        private Font _dayViewTimeFont;
        private bool _showArrowControls;
        private bool _showTodayButton;
        private bool _showDateInHeader;
        private TodayButton _btnToday;
        private NavigateLeftButton _btnLeft;
        private NavigateRightButton _btnRight;
        private bool _showingToolTip;
        private bool _showEventTooltips;
        private bool _loadPresetHolidays;
        private CalendarEvent _clickedEvent;
        private bool _showDisabledEvents;
        private bool _showDashedBorderOnDisabledEvents;
        private bool _dimDisabledEvents;
        private bool _highlightCurrentDay;
        private CalendarViews _calendarView;
        private readonly ScrollPanel _scrollPanel;

        private readonly List<IEvent> _events;
        private readonly List<Rectangle> _rectangles;
        private readonly Dictionary<int, Point> _calendarDays;
        private readonly List<CalendarEvent> _calendarEvents;
        private readonly EventToolTip _eventTip;
        private ContextMenuStrip _contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem _miProperties;
        private TodayButton _commit;

        private const int MarginSize = 20;
        
        /// <summary>
        /// Indicates the font for the times on the day view
        /// </summary>
        
        public Font DayViewTimeFont
        {
            get { return _dayViewTimeFont; }
            set
            {
                _dayViewTimeFont = value;
                if (_calendarView == CalendarViews.Day)
                    _scrollPanel.Refresh();
                else Refresh();
            }
        }

        /// <summary>
        /// Indicates the type of calendar to render, Month or Day view
        /// </summary>
        public CalendarViews CalendarView
        {
            get { return _calendarView; }
            set
            {
                _calendarView = value;
                _scrollPanel.Visible = value == CalendarViews.Day;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether today's date should be highlighted
        /// </summary>
        public bool HighlightCurrentDay
        {
            get { return _highlightCurrentDay; }
            set
            {
                _highlightCurrentDay = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether events can be right-clicked and edited
        /// </summary>
        public bool AllowEditingEvents
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether disabled events will appear as "dimmed".
        /// This property is only used if <see cref="ShowDisabledEvents"/> is set to true.
        /// </summary>
        public bool DimDisabledEvents
        {
            get { return _dimDisabledEvents; }
            set
            {
                _dimDisabledEvents = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates if a dashed border should show up around disabled events.
        /// This property is only used if <see cref="ShowDisabledEvents"/> is set to true.
        /// </summary>
        public bool ShowDashedBorderOnDisabledEvents
        {
            get { return _showDashedBorderOnDisabledEvents; }
            set
            {
                _showDashedBorderOnDisabledEvents = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether disabled events should show up on the calendar control
        /// </summary>
        public bool ShowDisabledEvents
        {
            get { return _showDisabledEvents; }
            set
            {
                _showDisabledEvents = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether Federal Holidays are automatically preloaded onto the calendar
        /// </summary>
        public bool LoadPresetHolidays
        {
            get { return _loadPresetHolidays; }
            set
            {
                _loadPresetHolidays = value;
                if (_loadPresetHolidays)
                {
                    _events.Clear();
                    Refresh();
                }
                else
                {
                    _events.Clear();
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Indicates whether hovering over an event will display a tooltip of the event
        /// </summary>
        public bool ShowEventTooltips
        {
            get { return _showEventTooltips; }
            set { _showEventTooltips = value; _eventTip.Visible = false; }
        }

        /// <summary>
        /// Get or Set this value to the Font you wish to use to render the date in the upper right corner
        /// </summary>
        public Font DateHeaderFont
        {
            get { return _dateHeaderFont; }
            set
            {
                _dateHeaderFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether the date should be displayed in the upper right hand corner of the calendar control
        /// </summary>
        public bool ShowDateInHeader
        {
            get { return _showDateInHeader; }
            set
            {
                _showDateInHeader = value;
                if (_calendarView == CalendarViews.Day)
                    ResizeScrollPanel();

                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether the calendar control should render the previous/next month buttons
        /// </summary>
        public bool ShowArrowControls
        {
            get { return _showArrowControls; }
            set
            {
                _showArrowControls = value;
                _btnLeft.Visible = value;
                _btnRight.Visible = value;
                if (_calendarView == CalendarViews.Day)
                    ResizeScrollPanel();
                Refresh();
            }
        }

        /// <summary>
        /// Indicates whether the calendar control should render the Today button
        /// </summary>
        public bool ShowTodayButton
        {
            get { return _showTodayButton; }
            set
            {
                _showTodayButton = value;
                _btnToday.Visible = value;
                if (_calendarView == CalendarViews.Day)
                    ResizeScrollPanel();
                Refresh();
            }
        }

        /// <summary>
        /// The font used to render the Today button
        /// </summary>
        public Font TodayFont
        {
            get { return _todayFont; }
            set
            {
                _todayFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// The font used to render the number days on the calendar
        /// </summary>
        public Font DaysFont
        {
            get { return _daysFont; }
            set
            {
                _daysFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// The font used to render the days of the week text
        /// </summary>
        public Font DayOfWeekFont
        {
            get { return _dayOfWeekFont; }
            set
            {
                _dayOfWeekFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// The Date that the calendar is currently showing
        /// </summary>
        public DateTime CalendarDate
        {
            get { return _calendarDate; }
            set
            {
                _calendarDate = value;
                Refresh();
            }
        }

        /// <summary>
        /// Calendar Constructor
        /// </summary>
        public Calendar()
        {
            InitializeComponent();
            _calendarDate = DateTime.Now;
            _dayOfWeekFont = new Font("Arial", 10, FontStyle.Regular);
            _daysFont = new Font("Arial", 10, FontStyle.Regular);
            _todayFont = new Font("Arial", 10, FontStyle.Bold);
            _dateHeaderFont = new Font("Arial", 12, FontStyle.Bold);
            _dayViewTimeFont = new Font("Arial", 10, FontStyle.Bold);
            _showArrowControls = true;
            _showDateInHeader = true;
            _showTodayButton = true;
            _showingToolTip = false;
            _clickedEvent = null;
            _showDisabledEvents = false;
            _showDashedBorderOnDisabledEvents = true;
            _dimDisabledEvents = true;
            AllowEditingEvents = true;
            _highlightCurrentDay = true;
            _calendarView = CalendarViews.Month;
            _scrollPanel = new ScrollPanel();

            _scrollPanel.RightButtonClicked += ScrollPanelRightButtonClicked;

            _events = new List<IEvent>();
            _rectangles = new List<Rectangle>();
            _calendarDays = new Dictionary<int, Point>();
            _calendarEvents = new List<CalendarEvent>();
            _showEventTooltips = true;
            _eventTip = new EventToolTip { Visible = false };

            Controls.Add(_eventTip);

            LoadPresetHolidays = true;
            _scrollPanel.Visible = false;
            Controls.Add(_scrollPanel);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._btnToday = new TodayButton();
            this._btnLeft = new NavigateLeftButton();
            this._btnRight = new NavigateRightButton();
            this._contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._miProperties = new System.Windows.Forms.ToolStripMenuItem();
            this._commit = new TodayButton();
            this._contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnToday
            // 
            this._btnToday.BackColor = System.Drawing.Color.Transparent;
            this._btnToday.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this._btnToday.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this._btnToday.ButtonFont = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._btnToday.ButtonText = "Today";
            this._btnToday.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(144)))), ((int)(((byte)(254)))));
            this._btnToday.HighlightBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this._btnToday.HighlightButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this._btnToday.Location = new System.Drawing.Point(19, 20);
            this._btnToday.Name = "_btnToday";
            this._btnToday.Size = new System.Drawing.Size(72, 29);
            this._btnToday.TabIndex = 0;
            this._btnToday.TextColor = System.Drawing.Color.Black;
            this._btnToday.ButtonClicked += new CoolButton.ButtonClickedArgs(this.BtnTodayButtonClicked);
            // 
            // _btnLeft
            // 
            this._btnLeft.BackColor = System.Drawing.Color.Transparent;
            this._btnLeft.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this._btnLeft.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this._btnLeft.ButtonFont = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._btnLeft.ButtonText = "<";
            this._btnLeft.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(144)))), ((int)(((byte)(254)))));
            this._btnLeft.HighlightBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this._btnLeft.HighlightButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this._btnLeft.Location = new System.Drawing.Point(98, 20);
            this._btnLeft.Name = "_btnLeft";
            this._btnLeft.Size = new System.Drawing.Size(42, 29);
            this._btnLeft.TabIndex = 1;
            this._btnLeft.TextColor = System.Drawing.Color.Black;
            this._btnLeft.ButtonClicked += new CoolButton.ButtonClickedArgs(this.BtnLeftButtonClicked);
            // 
            // _btnRight
            // 
            this._btnRight.BackColor = System.Drawing.Color.Transparent;
            this._btnRight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this._btnRight.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this._btnRight.ButtonFont = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._btnRight.ButtonText = ">";
            this._btnRight.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(144)))), ((int)(((byte)(254)))));
            this._btnRight.HighlightBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this._btnRight.HighlightButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this._btnRight.Location = new System.Drawing.Point(138, 20);
            this._btnRight.Name = "_btnRight";
            this._btnRight.Size = new System.Drawing.Size(42, 29);
            this._btnRight.TabIndex = 2;
            this._btnRight.TextColor = System.Drawing.Color.Black;
            this._btnRight.ButtonClicked += new CoolButton.ButtonClickedArgs(this.BtnRightButtonClicked);
            // 
            // _contextMenuStrip1
            // 
            this._contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miProperties});
            this._contextMenuStrip1.Name = "_contextMenuStrip1";
            this._contextMenuStrip1.Size = new System.Drawing.Size(214, 26);
            // 
            // _miProperties
            // 
            this._miProperties.Name = "_miProperties";
            this._miProperties.Size = new System.Drawing.Size(213, 22);
            this._miProperties.Text = "Created by Sidhant Mishra";
            this._miProperties.Click += new System.EventHandler(this.MenuItemPropertiesClick);
            // 
            // _commit
            // 
            this._commit.BackColor = System.Drawing.Color.Transparent;
            this._commit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this._commit.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this._commit.ButtonFont = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this._commit.ButtonText = "Commit";
            this._commit.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(144)))), ((int)(((byte)(254)))));
            this._commit.HighlightBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this._commit.HighlightButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this._commit.Location = new System.Drawing.Point(188, 20);
            this._commit.Name = "_commit";
            this._commit.Size = new System.Drawing.Size(72, 29);
            this._commit.TabIndex = 30;
            this._commit.TextColor = System.Drawing.Color.Black;
            this._commit.ButtonClicked += new CoolButton.ButtonClickedArgs(this.CommitClicked);
            // 
            // Calendar
            // 
            this.Controls.Add(this._commit);
            this.Controls.Add(this._btnRight);
            this.Controls.Add(this._btnLeft);
            this.Controls.Add(this._btnToday);
            this.DoubleBuffered = true;
            this.Name = "Calendar";
            this.Size = new System.Drawing.Size(512, 440);
            this.Load += new System.EventHandler(this.CalendarLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CalendarPaint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CalendarMouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CalendarMouseMove);
            this.Resize += new System.EventHandler(this.CalendarResize);
            this._contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Adds an event to the calendar
        /// </summary>
        /// <param name="calendarEvent">The <see cref="IEvent"/> to add to the calendar</param>
        public void AddEvent(IEvent calendarEvent)
        {
            _events.Add(calendarEvent);
            Refresh();
        }

        /// <summary>
        /// Removes an event from the calendar
        /// </summary>
        /// <param name="calendarEvent">The <see cref="IEvent"/> to remove to the calendar</param>
        public void RemoveEvent(IEvent calendarEvent)
        {
            _events.Remove(calendarEvent);
            Refresh();
        }

        private void CalendarLoad(object sender, EventArgs e)
        {
            if (Parent != null)
                Parent.Resize += ParentResize;
            ResizeScrollPanel();
        }

        private void CalendarPaint(object sender, PaintEventArgs e)
        {
            if (_showingToolTip)
                return;

            if (_calendarView == CalendarViews.Month)
                RenderMonthCalendar(e);
            if (_calendarView == CalendarViews.Day)
                RenderDayCalendar(e);
        }

        private void CalendarMouseMove(object sender, MouseEventArgs e)
        {
            if (!_showEventTooltips)
                return;

            int num = _calendarEvents.Count;
            for (int i = 0; i < num; i++)
            {
                var z = _calendarEvents[i];

                if ((z.EventArea.Contains(e.X, e.Y) && z.Event.TooltipEnabled && _calendarView == CalendarViews.Month) ||
                    (_calendarView == CalendarViews.Day && z.EventArea.Contains(e.X, e.Y + _scrollPanel.ScrollOffset) && z.Event.TooltipEnabled))
                {
                    _eventTip.ShouldRender = false;
                    _showingToolTip = true;
                    _eventTip.EventToolTipText = z.Event.EventText;
                    if (z.Event.IgnoreTimeComponent == false)
                        _eventTip.EventToolTipText += "\n" + z.Event.Date.ToShortTimeString();
                    _eventTip.Location = new Point(e.X + 5, e.Y - _eventTip.CalculateSize().Height);
                    _eventTip.ShouldRender = true;
                    _eventTip.Visible = true;

                    _showingToolTip = false;
                    return;
                }
            }

            _eventTip.Visible = false;
            _eventTip.ShouldRender = false;
        }

        private void ScrollPanelRightButtonClicked(object sender, MouseEventArgs e)
        {
            if (AllowEditingEvents && _calendarView == CalendarViews.Day)
            {
                int num = _calendarEvents.Count;
                for (int i = 0; i < num; i++)
                {
                    var z = _calendarEvents[i];

                    if (z.EventArea.Contains(e.X, e.Y + _scrollPanel.ScrollOffset) && !z.Event.ReadOnlyEvent)
                    {
                        _clickedEvent = z;
                        _contextMenuStrip1.Show(_scrollPanel, new Point(e.X, e.Y));
                        _eventTip.Visible = false;
                        _eventTip.ShouldRender = false;
                        break;
                    }
                }
            }
        }

        private void CalendarMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_calendarView == CalendarViews.Month)
                {
                    int num = _calendarEvents.Count;
                    
                    for (int i = 0; i < num; i++)
                    {
                        var z = _calendarEvents[i];

                        if (z.EventArea.Contains(e.X, e.Y) && !z.Event.ReadOnlyEvent)
                        {
                            _clickedEvent = z;
                            _contextMenuStrip1.Show(this, e.Location);
                            _eventTip.Visible = false;
                            _eventTip.ShouldRender = false;
                            break;
                        }
                    }
                }
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (_calendarView == CalendarViews.Month)
                {
                    var newEvent = new CustomEvent();                    
                    int num = _calendarEvents.Count;
                    int xBarrier = 189;
                    int yBarrier = (int)(Math.Ceiling(547.0 / numWeeks));         
                    Point editPoint = new Point(e.X-20, e.Y-75);
                    bool run = true;
                    bool run1 = true;
                    bool run2 = true;
                    int probableDay=-(int)(startWeekEnum)+1;

                    for (int i=0; i<num; i++)
                    {
                        var z = _calendarEvents[i];

                        if (z.EventArea.Contains(e.X, e.Y) && !z.Event.ReadOnlyEvent)
                        {
                            _clickedEvent = z;
                            _eventTip.Visible = false;
                            _eventTip.ShouldRender = false;                            
                            move = true;
                            return;
                        }                        
                    }

                    if (move)
                    {
                        move = false;
                        while (run)
                        {
                            if (editPoint.X > xBarrier)
                            {
                                editPoint.X -= xBarrier;
                                probableDay++;
                                run1 = false;
                            }
                            if (editPoint.Y > yBarrier)
                            {
                                editPoint.Y -= yBarrier;
                                probableDay += 7;
                                run2 = false;
                            }
                            if (run1 && run2)
                            {
                                run = false;
                            }
                            run1 = true;
                            run2 = true;
                        }
                        DateTime newDate = DateTime.Now;
                        try
                        {
                            newDate = new DateTime(_calendarDate.Year, _calendarDate.Month, probableDay);
                        }
                        catch
                        {
                            if (probableDay>0)
                            {
                                try
                                {
                                    newDate = new DateTime(_calendarDate.Year, _calendarDate.Month + 1, 
                                        probableDay - DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month));
                                }
                                catch
                                {
                                    newDate = new DateTime(_calendarDate.Year + 1, 1, 
                                        probableDay - DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month));
                                }
                            }
                            else
                            {
                                try
                                {
                                    newDate = new DateTime(_calendarDate.Year, _calendarDate.Month - 1, 
                                        DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month-1) + probableDay);
                                }                                
                                catch
                                {
                                    newDate = new DateTime(_calendarDate.Year - 1, 12, 31 + probableDay);
                                }
                            }
                        }
                        foreach (CustomEvent evnt in NetGlobals.customEvents)
                        {
                            if (evnt == _clickedEvent.Event)
                            {
                                newEvent = new CustomEvent
                                {
                                    Name = evnt.Name,
                                    EventText = evnt.EventText,
                                    IgnoreTimeComponent = true,
                                    Date = newDate,
                                    EventColor = evnt.EventColor,
                                    Order = evnt.Order,
                                    NumberOfEvents = evnt.NumberOfEvents,
                                };
                                invisEvent = new CustomEvent
                                {
                                    Name = evnt.Name,
                                    EventText = evnt.EventText,
                                    IgnoreTimeComponent = true,
                                    Date = evnt.Date,
                                    EventColor = Color.Black,
                                    Enabled = false,
                                    Order = evnt.Order,
                                    NumberOfEvents = evnt.NumberOfEvents,
                                    };
                                storeColor = evnt.EventColor;
                            }
                        }
                        DateTime oldReleaseDate;
                        if (newEvent.Date.DayOfWeek == 0 || (int)newEvent.Date.DayOfWeek == 6)
                        {
                            invisEvent.Enabled = true;
                            MessageBox.Show("Error: This date falls on a weekend");
                            return;
                        }
                        using (StreamReader sr = new StreamReader(@"TEMPDATA•" + (NetGlobals.docCounter) + ".txt"))
                        {
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                if (line.StartsWith(_clickedEvent.Event.Name + "•"))
                                {
                                    individualData = line.Split('•');
                                }
                            }
                            oldReleaseDate = new DateTime(int.Parse(individualData[individualData.Length-2].Split('/')[2]), 
                                int.Parse(individualData[individualData.Length-2].Split('/')[0]),
                                int.Parse(individualData[individualData.Length - 2].Split('/')[1]));
                            daysLost = -int.Parse(individualData[individualData.Length - 1]);
                        }

                        if (_clickedEvent.Event.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                        {
                            DateTime dcDate = DateTime.Now;
                            if (individualData[1] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                          
                            int store = int.Parse(individualData[3]);
                            individualData[3] = (int.Parse(individualData[3]) + Comparison(_clickedEvent.Event.Date, newEvent.Date)).ToString();
                            if (int.Parse(individualData[3]) < 0)
                            {
                                invisEvent.Enabled = true;
                                individualData[3] = store.ToString();
                                MessageBox.Show("This results in a negative duration (" + (int.Parse(individualData[3]) +
                                    Comparison(_clickedEvent.Event.Date, newEvent.Date)) + ") days long. Please try again.");
                                return;
                            }
                            NetGlobals.customEvents.Remove(_clickedEvent.Event);

                            NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                            {
                                if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                {
                                    dcDate = evnt.Date;
                                }
                            });

                            if (dcDate == newEvent.Date)
                            {
                                newEvent.Enabled = false;
                            }
                            else
                            {
                                newEvent.Enabled = true;
                            }                            
                            NetGlobals.customEvents.Add(newEvent);                            
                            individualData[1] = newEvent.Date.ToString("d",CultureInfo.InvariantCulture);
                            _events.Clear();
                            _events.AddRange(NetGlobals.customEvents);
                            Refresh();
                            saveTemp();
                        }
                        else if (_clickedEvent.Event.EventText == _clickedEvent.Event.Name + " Release Date")
                        {
                            DateTime newRelDate = newEvent.Date;
                            DateTime relDate = _clickedEvent.Event.Date;

                            if (individualData[individualData.Length - 4] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[individualData.Length - 4] = newEvent.Date.ToString("d",
                                CultureInfo.InvariantCulture);

                            currentDaysLost = Comparison(newRelDate, relDate);
                            daysLost += currentDaysLost;
                            individualData[individualData.Length - 2] = newRelDate.ToString("d",
                                CultureInfo.InvariantCulture);
                            individualData[individualData.Length - 1] = daysLost.ToString();
                            drawNewEvent = newEvent;

                            if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                            {
                                commit = true;
                                NetGlobals.customEvents.Add(newEvent);
                                NetGlobals.customEvents.Remove(_clickedEvent.Event);

                                NetGlobals.tempEvents.Clear();
                                NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                saveTemp();
                                Refresh();
                            }
                        }
                        else if (_clickedEvent.Event.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                        {
                            DateTime newDevDate = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime devECDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust1Date = DateTime.Now;
                            DateTime cust2Date = DateTime.Now;
                            DateTime cust3Date = DateTime.Now;
                            DateTime cust4Date = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;

                            IEvent cust1Event = null;
                            IEvent cust2Event = null; 
                            IEvent cust3Event = null;
                            IEvent cust4Event = null; 
                            IEvent cust5Event = null;
                            IEvent cust6Event = null;
                            IEvent relEvent = null;
                            IEvent devECEvent = null;

                            bool enabled;
                            if (individualData[2] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[2] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 0)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                });

                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                newRelDate = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[6] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[7] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 1)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[11] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[12] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawRelEvent.Date = newRelDate;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(devECEvent);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 2)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                cust2Date = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[16] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[17] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawCust2Event.Date = cust2Date;
                                drawRelEvent.Date = newRelDate;
                                
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(cust2Event);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newcust2Event = cust2Event;
                                    newcust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newcust2Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 3)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                cust2Date = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[21] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[22] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 4)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                cust2Date = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[26] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[27] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                cust2Date = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Dev Est Comp Date")
                                    {
                                        devECDate = evnt.Date;
                                        devECEvent = evnt;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Event = evnt;
                                        cust1Date = evnt.Date;
                                        originalCust1Event = evnt;
                                        drawOldCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust1Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                if (newDevDate == devECDate)
                                {
                                    enabled = false;
                                }
                                else
                                {
                                    enabled = true;
                                }
                                cust1Date = AddBusinessDays(newDevDate, int.Parse(individualData[4]));
                                individualData[7] = cust1Date.ToString("d", CultureInfo.InvariantCulture);
                                cust2Date = AddBusinessDays(cust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                cust6Date = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawCust1Event.Date = cust1Date;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date; 
                                drawCust6Event.Date = cust6Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(devECEvent);
                                    NetGlobals.customEvents.Remove(cust1Event);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newDevECEvent = devECEvent;
                                    newDevECEvent.Enabled = enabled;
                                    NetGlobals.customEvents.Add(newDevECEvent);

                                    IEvent newCust1Event = cust1Event;
                                    newCust1Event.Date = cust1Date;
                                    NetGlobals.customEvents.Add(newCust1Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            
                        }
                        else if (_clickedEvent.Event.Order == 2)
                        {
                            DateTime newCust1Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust2Date = DateTime.Now;
                            DateTime cust3Date = DateTime.Now;
                            DateTime cust4Date = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;
                            DateTime devCompDate = DateTime.Now;
                            
                            IEvent cust2Event = null;
                            IEvent cust3Event = null;
                            IEvent cust4Event = null;
                            IEvent cust5Event = null;
                            IEvent cust6Event = null;
                            IEvent relEvent = null;

                            string devName = "";

                            if (individualData[7] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[7] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 1)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before "+ devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[11] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[12] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }

                            else if (_clickedEvent.Event.NumberOfEvents == 2)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[16] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[17] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust2Event.Date = cust2Date;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust2Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }

                            else if (_clickedEvent.Event.NumberOfEvents == 3)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[21] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[22] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }

                            else if (_clickedEvent.Event.NumberOfEvents == 4)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[26] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[27] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }

                            else if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }

                            else if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.EventText == _clickedEvent.Event.Name + " Dev Comp Date")
                                    {
                                        devCompDate = evnt.Date;
                                        devName = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Event = evnt;
                                        cust2Date = evnt.Date;
                                        originalCust2Event = evnt;
                                        drawOldCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust2Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[4]);
                                individualData[4] = Comparison(newCust1Date, devCompDate).ToString();
                                if (int.Parse(individualData[4]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + devName);
                                    individualData[4] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                                individualData[12] = cust2Date.ToString("d", CultureInfo.InvariantCulture);
                                cust3Date = AddBusinessDays(cust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                cust6Date = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust2Event.Date = cust2Date;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;
                                drawCust6Event.Date = cust6Date;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust2Event);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newCust2Event = cust2Event;
                                    newCust2Event.Date = cust2Date;
                                    NetGlobals.customEvents.Add(newCust2Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                        else if (_clickedEvent.Event.Order == 3)
                        {
                            DateTime newCust2Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust1Date = DateTime.Now;
                            DateTime cust3Date = DateTime.Now;
                            DateTime cust4Date = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;

                            IEvent cust3Event = null;
                            IEvent cust4Event = null;
                            IEvent cust5Event = null;
                            IEvent cust6Event = null;
                            IEvent relEvent = null;

                            string cust1Name = "";

                            if (individualData[12] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[12] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 2)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Date = evnt.Date;
                                        cust1Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[9]);
                                individualData[9] = Comparison(newCust2Date, cust1Date).ToString();
                                if (int.Parse(individualData[9]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust1Name);
                                    individualData[9] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[16] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[17] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 3)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Date = evnt.Date;
                                        cust1Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[9]);
                                individualData[9] = Comparison(newCust2Date, cust1Date).ToString();
                                if (int.Parse(individualData[9]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust1Name);
                                    individualData[9] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust3Date = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[21] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[22] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust3Event.Date = cust3Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust3Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 4)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Date = evnt.Date;
                                        cust1Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[9]);
                                individualData[9] = Comparison(newCust2Date, cust1Date).ToString();
                                if (int.Parse(individualData[9]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust1Name);
                                    individualData[9] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust3Date = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[26] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[27] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Date = evnt.Date;
                                        cust1Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[9]);
                                individualData[9] = Comparison(newCust2Date, cust1Date).ToString();
                                if (int.Parse(individualData[9]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust1Name);
                                    individualData[9] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust3Date = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 2)
                                    {
                                        cust1Date = evnt.Date;
                                        cust1Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Event = evnt;
                                        cust3Date = evnt.Date;
                                        originalCust3Event = evnt;
                                        drawOldCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust3Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[9]);
                                individualData[9] = Comparison(newCust2Date, cust1Date).ToString();
                                if (int.Parse(individualData[9]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust1Name);
                                    individualData[9] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust3Date = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                individualData[17] = cust3Date.ToString("d", CultureInfo.InvariantCulture);
                                cust4Date = AddBusinessDays(cust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                cust6Date = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust3Event.Date = cust3Date;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;
                                drawCust6Event.Date = cust6Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust3Event);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event); 
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newCust3Event = cust3Event;
                                    newCust3Event.Date = cust3Date;
                                    NetGlobals.customEvents.Add(newCust3Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                        else if (_clickedEvent.Event.Order == 4)
                        {
                            DateTime newCust3Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust2Date = DateTime.Now;
                            DateTime cust4Date = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;

                            IEvent cust4Event = null;
                            IEvent cust5Event = null;
                            IEvent cust6Event = null;
                            IEvent relEvent = null;

                            string cust2Name = "";

                            if (individualData[17] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[17] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 3)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Date = evnt.Date;
                                        cust2Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[14]);
                                individualData[14] = Comparison(newCust3Date, cust2Date).ToString();
                                if (int.Parse(individualData[14]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust2Name);
                                    individualData[14] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += Comparison(newRelDate, relDate);
                                individualData[21] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[22] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 4)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Date = evnt.Date;
                                        cust2Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[14]);
                                individualData[14] = Comparison(newCust3Date, cust2Date).ToString();
                                if (int.Parse(individualData[14]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust2Name);
                                    individualData[14] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust4Date = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust4Date, int.Parse(individualData[24])); 
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[26] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[27] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust4Event.Date = cust4Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust4Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Date = evnt.Date;
                                        cust2Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[14]);
                                individualData[14] = Comparison(newCust3Date, cust2Date).ToString();
                                if (int.Parse(individualData[14]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust2Name);
                                    individualData[14] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust4Date = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            else if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 3)
                                    {
                                        cust2Date = evnt.Date;
                                        cust2Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Event = evnt;
                                        cust4Date = evnt.Date;
                                        originalCust4Event = evnt;
                                        drawOldCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust4Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[14]);
                                individualData[14] = Comparison(newCust3Date, cust2Date).ToString();
                                if (int.Parse(individualData[14]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust2Name);
                                    individualData[14] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust4Date = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                individualData[22] = cust4Date.ToString("d", CultureInfo.InvariantCulture);
                                cust5Date = AddBusinessDays(cust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                cust6Date = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust4Event.Date = cust4Date;
                                drawCust5Event.Date = cust5Date;
                                drawCust6Event.Date = cust6Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust4Event);
                                    NetGlobals.customEvents.Remove(cust5Event);
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newCust4Event = cust4Event;
                                    newCust4Event.Date = cust4Date;
                                    NetGlobals.customEvents.Add(newCust4Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                        else if (_clickedEvent.Event.Order == 5)
                        {
                            DateTime newCust4Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust3Date = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;

                            IEvent cust5Event = null;
                            IEvent cust6Event = null;
                            IEvent relEvent = null;

                            string cust3Name = "";

                            if (individualData[22] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[22] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 4)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Date = evnt.Date;
                                        cust3Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[19]);
                                individualData[19] = Comparison(newCust4Date, cust3Date).ToString();
                                if (int.Parse(individualData[19]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust3Name);
                                    individualData[19] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust4Date, int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[26] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[27] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Date = evnt.Date;
                                        cust3Name = evnt.Name;
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };

                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });

                                int storeInt = int.Parse(individualData[19]);
                                individualData[19] = Comparison(newCust4Date, cust3Date).ToString();
                                if (int.Parse(individualData[19]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust3Name);
                                    individualData[19] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust5Date = AddBusinessDays(newCust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust5Event.Date = cust5Date;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust5Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 4)
                                    {
                                        cust3Date = evnt.Date;
                                        cust3Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };

                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }

                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Event = evnt;
                                        cust5Date = evnt.Date;
                                        originalCust5Event = evnt;
                                        drawOldCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust5Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });

                                int storeInt = int.Parse(individualData[19]);
                                individualData[19] = Comparison(newCust4Date, cust3Date).ToString();
                                if (int.Parse(individualData[19]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust3Name);
                                    individualData[19] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust5Date = AddBusinessDays(newCust4Date, int.Parse(individualData[24]));
                                individualData[27] = cust5Date.ToString("d", CultureInfo.InvariantCulture);
                                cust6Date = AddBusinessDays(cust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34])); 
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust5Event.Date = cust5Date;
                                drawCust6Event.Date = cust6Date;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust5Event);
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newCust5Event = cust5Event;
                                    newCust5Event.Date = cust5Date;
                                    NetGlobals.customEvents.Add(newCust5Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                        else if (_clickedEvent.Event.Order == 6)
                        {
                            DateTime newCust5Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust4Date = DateTime.Now;
                            DateTime cust6Date = DateTime.Now;

                            IEvent cust6Event = null;
                            IEvent relEvent = null;

                            string cust4Name = "";

                            if (individualData[27] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[27] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 5)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Date = evnt.Date;
                                        cust4Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[24]);
                                individualData[24] = Comparison(newCust5Date, cust4Date).ToString();
                                if (int.Parse(individualData[24]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust4Name);
                                    individualData[24] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[31] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[32] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                            if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 5)
                                    {
                                        cust4Date = evnt.Date;
                                        cust4Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                    else if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 7)
                                    {
                                        cust6Event = evnt;
                                        cust6Date = evnt.Date;
                                        originalCust6Event = evnt;
                                        drawOldCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                        drawCust6Event = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[24]);
                                individualData[24] = Comparison(newCust5Date, cust4Date).ToString();
                                if (int.Parse(individualData[24]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust4Name);
                                    individualData[24] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                cust6Date = AddBusinessDays(newCust5Date, int.Parse(individualData[29]));
                                individualData[32] = cust6Date.ToString("d", CultureInfo.InvariantCulture);
                                newRelDate = AddBusinessDays(cust6Date, int.Parse(individualData[34])); 
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;
                                drawCust6Event.Date = cust6Date;
                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);
                                    NetGlobals.customEvents.Remove(cust6Event);

                                    IEvent newCust6Event = cust6Event;
                                    newCust6Event.Date = cust6Date;
                                    NetGlobals.customEvents.Add(newCust6Event);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                        else if (_clickedEvent.Event.Order == 7)
                        {
                            DateTime newCust6Date = newEvent.Date;
                            DateTime newRelDate = DateTime.Now;
                            DateTime relDate = DateTime.Now;
                            DateTime cust5Date = DateTime.Now;

                            IEvent relEvent = null;

                            string cust5Name = "";

                            if (individualData[32] == newEvent.Date.ToString("d", CultureInfo.InvariantCulture))
                            {
                                invisEvent.Enabled = true;
                                MessageBox.Show("Nothing has changed");
                                _clickedEvent = null;
                                return;
                            }
                            individualData[32] = newEvent.Date.ToString("d", CultureInfo.InvariantCulture);

                            if (_clickedEvent.Event.NumberOfEvents == 6)
                            {
                                NetGlobals.customEvents.ForEach(delegate(IEvent evnt)
                                {
                                    if (evnt.Name == _clickedEvent.Event.Name && evnt.Order == 6)
                                    {
                                        cust5Date = evnt.Date;
                                        cust5Name = evnt.Name;
                                    }
                                    else if (evnt.EventText == _clickedEvent.Event.Name + " Release Date")
                                    {
                                        relEvent = evnt;
                                        relDate = evnt.Date;
                                        originalRel = evnt;
                                        drawOldRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = Color.Black,
                                            Enabled = false,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                        drawRelEvent = new CustomEvent
                                        {
                                            Name = evnt.Name,
                                            EventText = evnt.EventText,
                                            IgnoreTimeComponent = true,
                                            Date = evnt.Date,
                                            EventColor = evnt.EventColor,
                                            Order = evnt.Order,
                                            NumberOfEvents = evnt.NumberOfEvents,
                                        };
                                    }
                                });
                                int storeInt = int.Parse(individualData[29]);
                                individualData[29] = Comparison(newCust6Date, cust5Date).ToString();
                                if (int.Parse(individualData[29]) < 0)
                                {
                                    invisEvent.Enabled = true;
                                    MessageBox.Show("This event cannot be before " + cust5Name);
                                    individualData[29] = storeInt.ToString();
                                    _clickedEvent = null;
                                    return;
                                }
                                newRelDate = AddBusinessDays(newCust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                currentDaysLost = Comparison(newRelDate, relDate);
                                daysLost += currentDaysLost;
                                individualData[36] = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[37] = daysLost.ToString();
                                drawNewEvent = newEvent;
                                drawRelEvent.Date = newRelDate;

                                if (Checker(oldReleaseDate, Comparison(newRelDate, relDate), _clickedEvent.Event))
                                {
                                    commit = true;
                                    NetGlobals.customEvents.Add(newEvent);
                                    NetGlobals.customEvents.Remove(_clickedEvent.Event);
                                    NetGlobals.customEvents.Remove(relEvent);

                                    IEvent newRelEvent = relEvent;
                                    newRelEvent.Date = newRelDate;
                                    NetGlobals.customEvents.Add(newRelEvent);

                                    NetGlobals.tempEvents.Clear();
                                    NetGlobals.tempEvents.AddRange(NetGlobals.customEvents);

                                    saveTemp();
                                    Refresh();
                                }
                            }
                        }
                    }                    
                }                
            }
        }

        private void saveTemp ()
        {
            NetGlobals.docCounter++;
            string[] linesInPath = File.ReadAllLines(@"TEMPDATA•"+(NetGlobals.docCounter-1)+".txt");
            for (int i = 0; i < linesInPath.Length; i++)
            {
                if (linesInPath[i].StartsWith(_clickedEvent.Event.Name + "•"))
                {
                    linesInPath[i] = individualData[0] + "•" + individualData[1] + "•" + individualData[2] + "•" + individualData[3] + "•" 
                        + individualData[4] + "•";
                    if (individualData.Length > 9)
                    {
                        linesInPath[i] += individualData[5] + "•" + individualData[6] + "•" + individualData[7] + "•" +
                                          individualData[8] + "•" + individualData[9] + "•";
                    }
                    if (individualData.Length > 14)
                    {
                        linesInPath[i] += individualData[10] + "•" + individualData[11] + "•" + individualData[12] + "•" +
                                          individualData[13] + "•" + individualData[14] + "•";
                    }
                    if (individualData.Length > 19)
                    {
                        linesInPath[i] += individualData[15] + "•" + individualData[16] + "•" + individualData[17] + "•" +
                                          individualData[18] + "•" + individualData[19] + "•";
                    }
                    if (individualData.Length > 24)
                    {
                        linesInPath[i] += individualData[20] + "•" + individualData[21] + "•" + individualData[22] + "•" +
                                          individualData[23] + "•" + individualData[24] + "•";
                    }
                    if (individualData.Length > 29)
                    {
                        linesInPath[i] += individualData[25] + "•" + individualData[26] + "•" + individualData[27] + "•" +
                                          individualData[28] + "•" + individualData[29] + "•";
                    }
                    if (individualData.Length > 34)
                    {
                        linesInPath[i] += individualData[30] + "•" + individualData[31] + "•" + individualData[32] + "•" +
                                          individualData[33] + "•" + individualData[34] + "•";
                    }
                    linesInPath[i] += individualData[individualData.Length - 3] + "•" +
                                      individualData[individualData.Length - 2] + "•" +
                                      (-int.Parse(individualData[individualData.Length - 1]));
                }
            }
            File.WriteAllLines(@"TEMPDATA•" + NetGlobals.docCounter + ".txt", linesInPath);
        }

        private void CommitClicked (object sender)
        {
            if (commit)
            {
                _events.Clear();
                _events.AddRange(NetGlobals.tempEvents);
                _events.Remove(drawOldRelEvent);
                _events.Remove(drawRelEvent);
                _events.Remove(invisEvent);
                Refresh();
                commit = false;
            }
        }

        private void BtnTodayButtonClicked(object sender)
        {
            _calendarDate = DateTime.Now;
            Refresh();
        }

        private void BtnLeftButtonClicked(object sender)
        {
            if (_calendarView == CalendarViews.Month)
                _calendarDate = _calendarDate.AddMonths(-1);
            else if (_calendarView == CalendarViews.Day)
                _calendarDate = _calendarDate.AddDays(-1);
            Refresh();
        }

        private void BtnRightButtonClicked(object sender)
        {
            if (_calendarView == CalendarViews.Day)
                _calendarDate = _calendarDate.AddDays(1);
            else if (_calendarView == CalendarViews.Month)
                _calendarDate = _calendarDate.AddMonths(1);
            Refresh();
        }

        private void MenuItemPropertiesClick(object sender, EventArgs e)
        {           
            return;          
        }        
        public bool Checker (DateTime oldRelDate, int daysLost, IEvent eventt)
        {
            _events.Add(invisEvent);
            _events.Add(drawNewEvent); 
            _events.Add(drawOldCust1Event);
            _events.Add(drawCust1Event);
            _events.Add(drawOldCust2Event);
            _events.Add(drawCust2Event);
            _events.Add(drawOldCust3Event);
            _events.Add(drawCust3Event);
            _events.Add(drawOldCust4Event);
            _events.Add(drawCust4Event);
            _events.Add(drawOldCust5Event);
            _events.Add(drawCust5Event);
            _events.Add(drawOldCust6Event);
            _events.Add(drawCust6Event);
            _events.Add(drawOldRelEvent);
            _events.Add(drawRelEvent);
            _events.Remove(originalCust1Event); 
            _events.Remove(originalCust2Event); 
            _events.Remove(originalCust3Event);
            _events.Remove(originalCust4Event); 
            _events.Remove(originalCust5Event);
            _events.Remove(originalCust6Event);
            _events.Remove(originalRel);
            _events.Remove(_clickedEvent.Event);
            Refresh();            
            DialogResult result1;
            if (oldRelDate != new DateTime(int.Parse(individualData[6].Split('/')[2]), 
                int.Parse(individualData[6].Split('/')[0]), int.Parse(individualData[6].Split('/')[1])))
            {
                if (daysLost == 1)
                {
                    result1 = MessageBox.Show("This will move the Release Date later by " + daysLost + " day. Pressing OK will allow " +
                        "you to view and commit changes.", "Are you sure?", MessageBoxButtons.OKCancel);
                }
                else if (daysLost == -1)
                {
                    result1 = MessageBox.Show("This will move the Release Date earlier by " + (-daysLost) + " day. Pressing OK will allow " +
                        "you to view and commit changes.", "Are you sure?", MessageBoxButtons.OKCancel);
                }
                else if (daysLost < 0)
                {
                    result1 = MessageBox.Show("This will move the Release Date earlier by " + (-daysLost) + " days. Pressing OK will allow " +
                        "you to view and commit changes.", "Are you sure?", MessageBoxButtons.OKCancel);
                }
                else
                {
                    result1 = MessageBox.Show("This will move the Release Date later by " + daysLost + " days. Pressing OK will allow " +
                        "you to view and commit changes.", "Are you sure?", MessageBoxButtons.OKCancel);
                }
                if (result1 == DialogResult.Cancel)
                {
                    _events.Add(originalRel);
                    _events.Add(originalCust1Event);
                    _events.Add(originalCust2Event);
                    _events.Add(originalCust3Event);
                    _events.Add(originalCust4Event);
                    _events.Add(originalCust5Event);
                    _events.Add(originalCust6Event);
                    NetGlobals.customEvents.Clear();
                    NetGlobals.customEvents.AddRange(_events);
                    invisEvent.Enabled = true;
                    invisEvent.EventColor = storeColor;
                    NetGlobals.customEvents.Remove(drawOldCust1Event);
                    NetGlobals.customEvents.Remove(drawCust1Event);
                    NetGlobals.customEvents.Remove(drawOldCust2Event);
                    NetGlobals.customEvents.Remove(drawCust2Event);
                    NetGlobals.customEvents.Remove(drawOldCust3Event);
                    NetGlobals.customEvents.Remove(drawCust3Event);
                    NetGlobals.customEvents.Remove(drawOldCust4Event);
                    NetGlobals.customEvents.Remove(drawCust4Event);
                    NetGlobals.customEvents.Remove(drawOldCust5Event);
                    NetGlobals.customEvents.Remove(drawCust5Event);
                    NetGlobals.customEvents.Remove(drawOldCust6Event);
                    NetGlobals.customEvents.Remove(drawCust6Event);
                    NetGlobals.customEvents.Remove(drawNewEvent);
                    NetGlobals.customEvents.Remove(drawOldRelEvent);
                    NetGlobals.customEvents.Remove(drawRelEvent);
                    
                    _events.Clear();
                    _events.AddRange(NetGlobals.customEvents);
                    Refresh();
                    return false;
                }

                if (currentDaysLost > 0)
                {
                    if (int.Parse(individualData[individualData.Length-1]) > 0)
                    {
                        MessageBox.Show("Days Lost: " + currentDaysLost + ", Total Days Lost: " + (int.Parse(individualData[individualData.Length - 1])));
                        return true;
                    }
                    MessageBox.Show("Days Lost: " + currentDaysLost + ", Total Days Gained: " + (-int.Parse(individualData[individualData.Length - 1])));
                    return true;
                }
                   
                if (int.Parse(individualData[individualData.Length - 1]) < 0)
                {
                    MessageBox.Show("Days Gained: " + (-currentDaysLost) + ", Total Days Gained: " + (-int.Parse(individualData[individualData.Length - 1])));
                    return true;
                }
                    
                MessageBox.Show("Days Gained: " + (-currentDaysLost) + ", Total Days Lost: " + (int.Parse(individualData[individualData.Length - 1])));
                return true;
            }
            
            if (currentDaysLost > 0)
            {
                if (int.Parse(individualData[individualData.Length - 1]) > 0)
                {
                    MessageBox.Show("Days Lost: " + currentDaysLost + ", Total Days Lost: " + (int.Parse(individualData[individualData.Length - 1])));
                    return true;
                }
                MessageBox.Show("Days Lost: " + currentDaysLost + ", Total Days Gained: " + (-int.Parse(individualData[individualData.Length - 1])));
                return true;
            }

            if (int.Parse(individualData[individualData.Length - 1]) < 0)
            {
                MessageBox.Show("Days Gained: " + (-currentDaysLost) + ", Total Days Gained: " + (-int.Parse(individualData[individualData.Length - 1])));
                return true;
            }
            MessageBox.Show("Days Gained: " + (-currentDaysLost) + ", Total Days Lost: " + (int.Parse(individualData[individualData.Length - 1])));
            return true;
        }

        public int Comparison(DateTime endD, DateTime startD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 - (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return (int)(calcBusinessDays - 1.0);
        }

        public DateTime AddBusinessDays(DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday && newDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        private void ParentResize(object sender, EventArgs e)
        {
            ResizeScrollPanel();
            Refresh();
        }

        private void PresetHolidays()
        {        
            var newYears = new HolidayEvent
            {
                Date = new DateTime(DateTime.Now.Year, 1, 1),
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "New Years Day"
            };
            AddEvent(newYears);

            DateTime familyDate = new DateTime (DateTime.Now.Year, 2, 15);
            while (familyDate.DayOfWeek != DayOfWeek.Monday)
            {
                familyDate.AddDays(1);
            }
            var familyDay = new HolidayEvent
            {
                Date = familyDate,
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Family Day",

            };
            AddEvent(familyDay);

            DateTime victoriaDate = new DateTime(DateTime.Now.Year, 5, 18);
            while (victoriaDate.DayOfWeek != DayOfWeek.Monday)
            {
                victoriaDate.AddDays(1);
            }
            var victoriaDay = new HolidayEvent
            {
                Date = victoriaDate,
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Victoria Day",                
            };
            AddEvent(victoriaDay);

            DateTime civicDate = new DateTime(DateTime.Now.Year, 8, 1);
            while (civicDate.DayOfWeek != DayOfWeek.Monday)
            {
                civicDate.AddDays(1);
            }
            var civicHoliday = new HolidayEvent
            {
                Date = civicDate,
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Civic Holiday",
            };
            AddEvent(victoriaDay);

            DateTime labourDate = new DateTime(DateTime.Now.Year, 9, 1);
            while (labourDate.DayOfWeek != DayOfWeek.Monday)
            {
                labourDate.AddDays(1);
            }
            var labourDay = new HolidayEvent
            {
                Date = labourDate,
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Labour Day",
            };
            AddEvent(labourDay);

            DateTime thanksgivingDate = new DateTime(DateTime.Now.Year, 10, 8);
            while (thanksgivingDate.DayOfWeek != DayOfWeek.Monday)
            {
                thanksgivingDate.AddDays(1);
            }
            var thanksgiving = new HolidayEvent
            {
                Date = thanksgivingDate,
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Thanksgiving"
            };
            AddEvent(thanksgiving);
            
            var canadaDay = new HolidayEvent
            {
                Date = new DateTime(DateTime.Now.Year, 7, 1),
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Canada Day"
            };
            AddEvent(canadaDay);

            var veteransDay = new HolidayEvent
            {
                Date = new DateTime(DateTime.Now.Year, 11, 11),
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Remembrance Day"
            };
            AddEvent(veteransDay);

            var christmas = new HolidayEvent
            {
                Date = new DateTime(DateTime.Now.Year, 12, 25),
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Christmas Day"
            };
            AddEvent(christmas);

            var boxingDay = new HolidayEvent
            {
                Date = new DateTime(DateTime.Now.Year, 12, 26),
                RecurringFrequency = RecurringFrequencies.Yearly,
                EventText = "Boxing Day"
            };
            AddEvent(boxingDay);
        }        

        private DateTime LastDayOfWeekInMonth(DateTime day, DayOfWeek dow)
        {
            DateTime lastDay = new DateTime(day.Year, day.Month, 1).AddMonths(1).AddDays(-1);
            DayOfWeek lastDow = lastDay.DayOfWeek;

            int diff = dow - lastDow;

            if (diff > 0) diff -= 7;

            System.Diagnostics.Debug.Assert(diff <= 0);

            return lastDay.AddDays(diff);
        }

        private int Max(params float[] value)
        {
            return (int)value.Max(i => Math.Ceiling(i));
        }

        private bool DayForward(IEvent evnt, DateTime day)
        {
            if (evnt.ThisDayForwardOnly)
            {
                int c = DateTime.Compare(day, evnt.Date);

                if (c >= 0)
                    return true;

                return false;
            }

            return true;
        }

        internal Bitmap RequestImage()
        {
            const int cellHourWidth = 60;
            const int cellHourHeight = 30;
            var bmp = new Bitmap(ClientSize.Width, cellHourWidth * 24);
            Graphics g = Graphics.FromImage(bmp);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            var dt = new DateTime(_calendarDate.Year, _calendarDate.Month, _calendarDate.Day, 0, 0, 0);
            int xStart = 0;
            int yStart = 0;

            g.DrawRectangle(Pens.Black, 0, 0, ClientSize.Width - MarginSize * 2 - 2, cellHourHeight * 24);
            for (int i = 0; i < 24; i++)
            {
                var textWidth = (int)g.MeasureString(dt.ToString("htt").ToLower(), _dayViewTimeFont).Width;
                g.DrawRectangle(Pens.Black, xStart, yStart, cellHourWidth, cellHourHeight);
                g.DrawLine(Pens.Black, xStart + cellHourWidth, yStart + cellHourHeight,
                           ClientSize.Width - MarginSize * 2 - 3, yStart + cellHourHeight);
                g.DrawLine(Pens.DarkGray, xStart + cellHourWidth, yStart + cellHourHeight / 2,
                           ClientSize.Width - MarginSize * 2 - 3, yStart + cellHourHeight / 2);

                g.DrawString(dt.ToString("htt").ToLower(), _dayViewTimeFont, Brushes.Black, xStart + cellHourWidth - textWidth, yStart);
                yStart += cellHourHeight;
                dt = dt.AddHours(1);
            }

            dt = new DateTime(_calendarDate.Year, _calendarDate.Month, _calendarDate.Day, 23, 59, 0);

            List<IEvent> evnts = _events.Where(evnt => NeedsRendering(evnt, dt)).ToList().OrderBy(d => d.Date).ToList();

            xStart = cellHourWidth + 1;
            yStart = 0;

            g.Clip = new Region(new Rectangle(0, 0, ClientSize.Width - MarginSize * 2 - 2, cellHourHeight * 24));
            _calendarEvents.Clear();
            for (int i = 0; i < 24; i++)
            {
                dt = new DateTime(_calendarDate.Year, _calendarDate.Month, _calendarDate.Day, 0, 0, 0);
                dt = dt.AddHours(i);
                foreach (var evnt in evnts)
                {
                    TimeSpan ts = TimeSpan.FromHours(evnt.EventLengthInHours);

                    if (evnt.Date.Ticks >= dt.Ticks && evnt.Date.Ticks < dt.Add(ts).Ticks && evnt.EventLengthInHours > 0 && i >= evnt.Date.Hour)
                    {
                        int divisor = evnt.Date.Minute == 0 ? 1 : 60 / evnt.Date.Minute;
                        Color clr = Color.FromArgb(175, evnt.EventColor.R, evnt.EventColor.G, evnt.EventColor.B);
                        g.FillRectangle(new SolidBrush(GetFinalBackColor()), xStart, yStart + cellHourHeight / divisor + 1, ClientSize.Width 
                            - MarginSize * 2 - cellHourWidth - 3, cellHourHeight * ts.Hours - 1);
                        g.FillRectangle(new SolidBrush(clr), xStart, yStart + cellHourHeight / divisor + 1, ClientSize.Width - MarginSize * 2 
                            - cellHourWidth - 3, cellHourHeight * ts.Hours - 1);
                        g.DrawString(evnt.EventText, evnt.EventFont, new SolidBrush(evnt.EventTextColor), xStart, yStart + cellHourHeight / divisor);

                        var ce = new CalendarEvent
                                     {
                                         Event = evnt,
                                         Date = dt,
                                         EventArea = new Rectangle(xStart, yStart + cellHourHeight / divisor + 1,
                                                                   ClientSize.Width - MarginSize * 2 - cellHourWidth - 3,
                                                                   cellHourHeight * ts.Hours)
                                     };
                        _calendarEvents.Add(ce);
                    }
                }
                yStart += cellHourHeight;
            }

            g.Dispose();
            return bmp;
        }

        private Color GetFinalBackColor()
        {
            Control c = this;

            while (c != null)
            {
                if (c.BackColor != Color.Transparent)
                    return c.BackColor;
                c = c.Parent;
            }

            return Color.Transparent;
        }

        private void ResizeScrollPanel()
        {
            int controlsSpacing = ((!_showTodayButton) && (!_showDateInHeader) && (!_showArrowControls)) ? 0 : 30;

            _scrollPanel.Location = new Point(MarginSize, MarginSize + controlsSpacing);
            _scrollPanel.Size = new Size(ClientSize.Width - MarginSize * 2 - 1, ClientSize.Height - MarginSize - 1 - controlsSpacing);
        }

        private void RenderDayCalendar(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (_showDateInHeader)
            {
                SizeF dateHeaderSize = g.MeasureString(
                    _calendarDate.ToString("MMMM") + " " + _calendarDate.Day.ToString(CultureInfo.InvariantCulture) +
                    ", " + _calendarDate.Year.ToString(CultureInfo.InvariantCulture), DateHeaderFont);

                g.DrawString(
                    _calendarDate.ToString("MMMM") + " " + _calendarDate.Day.ToString(CultureInfo.InvariantCulture) +
                    ", " + _calendarDate.Year.ToString(CultureInfo.InvariantCulture),
                    _dateHeaderFont, Brushes.Black, ClientSize.Width - MarginSize - dateHeaderSize.Width,
                    MarginSize);
            }
        }

        private void RenderMonthCalendar(PaintEventArgs e)
        {
            _calendarDays.Clear();
            _calendarEvents.Clear();
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            SizeF sunSize = g.MeasureString("Sun", _dayOfWeekFont);
            SizeF monSize = g.MeasureString("Mon", _dayOfWeekFont);
            SizeF tueSize = g.MeasureString("Tue", _dayOfWeekFont);
            SizeF wedSize = g.MeasureString("Wed", _dayOfWeekFont);
            SizeF thuSize = g.MeasureString("Thu", _dayOfWeekFont);
            SizeF friSize = g.MeasureString("Fri", _dayOfWeekFont);
            SizeF satSize = g.MeasureString("Sat", _dayOfWeekFont);
            SizeF dateHeaderSize = g.MeasureString(
                _calendarDate.ToString("MMMM") + " " + _calendarDate.Year.ToString(CultureInfo.InvariantCulture), _dateHeaderFont);
            int headerSpacing = Max(sunSize.Height, monSize.Height, tueSize.Height, wedSize.Height, thuSize.Height, friSize.Height,
                          satSize.Height) + 5;
            int controlsSpacing = ((!_showTodayButton) && (!_showDateInHeader) && (!_showArrowControls)) ? 0 : 30;
            int cellWidth = (ClientSize.Width - MarginSize * 2) / 7;
            numWeeks = NumberOfWeeks(_calendarDate.Year, _calendarDate.Month);
            int cellHeight = (ClientSize.Height - MarginSize * 2 - headerSpacing - controlsSpacing) / numWeeks;
            int xStart = MarginSize;
            int yStart = MarginSize;
            startWeekEnum = new DateTime(_calendarDate.Year, _calendarDate.Month, 1).DayOfWeek;
            int startWeek = ((int)startWeekEnum) + 1;
            int rogueDays = startWeek - 1;

            yStart += headerSpacing + controlsSpacing;

            int counter = 1;
            int counter2 = 1;

            bool first = false;
            bool first2 = false;

            _btnToday.Location = new Point(MarginSize, MarginSize);

            for (int y = 0; y < numWeeks; y++)
            {
                for (int x = 0; x < 7; x++)
                {                    
                    if (rogueDays == 0 && counter <= DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month))
                    {
                        if (!_calendarDays.ContainsKey(counter))
                            _calendarDays.Add(counter, new Point(xStart, (int)(yStart + 2f + 
                                g.MeasureString(counter.ToString(CultureInfo.InvariantCulture), _daysFont).Height)));

                        if (_calendarDate.Year == DateTime.Now.Year && _calendarDate.Month == DateTime.Now.Month
                         && counter == DateTime.Now.Day && _highlightCurrentDay)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb(234, 234, 234)), xStart, yStart, cellWidth, cellHeight);
                        }

                        if (first == false)
                        {
                            first = true;
                            if (_calendarDate.Year == DateTime.Now.Year && _calendarDate.Month == DateTime.Now.Month
                         && counter == DateTime.Now.Day)
                            {
                                g.DrawString(
                                    _calendarDate.ToString("MMM") + " " + counter.ToString(CultureInfo.InvariantCulture),
                                    _todayFont, Brushes.Black, xStart + 5, yStart + 2);
                            }
                            else
                            {
                                g.DrawString(
                                    _calendarDate.ToString("MMM") + " " + counter.ToString(CultureInfo.InvariantCulture),
                                    _daysFont, Brushes.Black, xStart + 5, yStart + 2);
                            }
                        }
                        else
                        {
                            if (_calendarDate.Year == DateTime.Now.Year && _calendarDate.Month == DateTime.Now.Month
                         && counter == DateTime.Now.Day)
                            {
                                g.DrawString(counter.ToString(CultureInfo.InvariantCulture), _todayFont, Brushes.Black, xStart + 5, yStart + 2);
                            }
                            else
                            {
                                g.DrawString(counter.ToString(CultureInfo.InvariantCulture), _daysFont, Brushes.Black, xStart + 5, yStart + 2);
                            }
                        }
                        counter++;
                    }
                    else if (rogueDays > 0)
                    {
                        int dm =
                            DateTime.DaysInMonth(_calendarDate.AddMonths(-1).Year, _calendarDate.AddMonths(-1).Month) -
                            rogueDays + 1;
                        g.DrawString(dm.ToString(CultureInfo.InvariantCulture), _daysFont, new SolidBrush(Color.FromArgb(170, 170, 170)), xStart + 5, yStart + 2);
                        rogueDays--;
                    }

                    g.DrawRectangle(Pens.DarkGray, xStart, yStart, cellWidth, cellHeight);
                    if (rogueDays == 0 && counter > DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month))
                    {
                        if (first2 == false)
                            first2 = true;
                        else
                        {
                            if (counter2 == 1)
                            {
                                g.DrawString(_calendarDate.AddMonths(1).ToString("MMM") + " " + counter2.ToString(CultureInfo.InvariantCulture), _daysFont,
                                             new SolidBrush(Color.FromArgb(170, 170, 170)), xStart + 5, yStart + 2);
                            }
                            else
                            {
                                g.DrawString(counter2.ToString(CultureInfo.InvariantCulture), _daysFont,
                                             new SolidBrush(Color.FromArgb(170, 170, 170)), xStart + 5, yStart + 2);
                            }
                            counter2++;
                        }
                    }
                    xStart += cellWidth;
                }
                xStart = MarginSize;
                yStart += cellHeight;
            }
            xStart = MarginSize + ((cellWidth - (int)sunSize.Width) / 2);
            yStart = MarginSize + controlsSpacing;

            g.DrawString("Sun", _dayOfWeekFont, Brushes.Black, xStart, yStart);
            xStart = MarginSize + ((cellWidth - (int)monSize.Width) / 2) + cellWidth;
            g.DrawString("Mon", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            xStart = MarginSize + ((cellWidth - (int)tueSize.Width) / 2) + cellWidth * 2;
            g.DrawString("Tue", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            xStart = MarginSize + ((cellWidth - (int)wedSize.Width) / 2) + cellWidth * 3;
            g.DrawString("Wed", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            xStart = MarginSize + ((cellWidth - (int)thuSize.Width) / 2) + cellWidth * 4;
            g.DrawString("Thu", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            xStart = MarginSize + ((cellWidth - (int)friSize.Width) / 2) + cellWidth * 5;
            g.DrawString("Fri", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            xStart = MarginSize + ((cellWidth - (int)satSize.Width) / 2) + cellWidth * 6;
            g.DrawString("Sat", _dayOfWeekFont, Brushes.Black, xStart, yStart);

            if (_showDateInHeader)
            {
                g.DrawString(
                    _calendarDate.ToString("MMMM") + " " + _calendarDate.Year.ToString(CultureInfo.InvariantCulture),
                    _dateHeaderFont, Brushes.Black, ClientSize.Width - MarginSize - dateHeaderSize.Width,
                    MarginSize);
            }

            _events.Sort(new EventComparer());

            for (int i = 1; i <= DateTime.DaysInMonth(_calendarDate.Year, _calendarDate.Month); i++)
            {
                int renderOffsetY = 0;

                foreach (IEvent v in _events)
                {
                    var dt = new DateTime(_calendarDate.Year,_calendarDate.Month, i, 23, 59, _calendarDate.Second);
                    if (NeedsRendering(v, dt))
                    {
                        int alpha = 255;
                        if (!v.Enabled && _dimDisabledEvents)
                            alpha = 64;
                        Color alphaColor = Color.FromArgb(alpha, v.EventColor.R, v.EventColor.G, v.EventColor.B);

                        int offsetY = renderOffsetY;
                        Region r = g.Clip;
                        Point point = _calendarDays[i];
                        SizeF sz = g.MeasureString(v.EventText, v.EventFont);
                        int yy = point.Y - 1;
                        int xx = ((cellWidth - (int)sz.Width) / 2) + point.X;

                        if (sz.Width > cellWidth)
                            xx = point.X;
                        if (renderOffsetY + sz.Height > cellHeight - 10)
                            continue;
                        g.Clip = new Region(new Rectangle(point.X + 1, point.Y + offsetY, cellWidth - 1, (int)sz.Height));
                        g.FillRectangle(new SolidBrush(alphaColor), point.X + 1, point.Y + offsetY, cellWidth - 1, sz.Height);
                        if (!v.Enabled && _showDashedBorderOnDisabledEvents)
                        {
                            var p = new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 0))) { DashStyle = DashStyle.Dash };
                            g.DrawRectangle(p, point.X + 1, point.Y + offsetY, cellWidth - 2, sz.Height - 1);
                        }
                        g.DrawString(v.EventText, v.EventFont, new SolidBrush(v.EventTextColor), xx, yy + offsetY);
                        g.Clip = r;

                        var ev = new CalendarEvent
                        {
                            EventArea =
                                new Rectangle(point.X + 1, point.Y + offsetY, cellWidth - 1,
                                              (int)sz.Height),
                            Event = v,
                            Date = dt
                        };

                        _calendarEvents.Add(ev);
                        renderOffsetY += (int)sz.Height + 1;
                    }
                }
            }
            _rectangles.Clear();

            g.Dispose();
            e.Graphics.DrawImage(bmp, 0, 0, ClientSize.Width, ClientSize.Height);
            bmp.Dispose();
        }

        private bool NeedsRendering(IEvent evnt, DateTime day)
        {
            if (!evnt.Enabled && !_showDisabledEvents)
                return false;

            DayOfWeek dw = evnt.Date.DayOfWeek;

            if (evnt.RecurringFrequency == RecurringFrequencies.Daily)
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.Weekly && day.DayOfWeek == dw)
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryWeekend && (day.DayOfWeek == DayOfWeek.Saturday ||
                day.DayOfWeek == DayOfWeek.Sunday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryMonWedFri && (day.DayOfWeek == DayOfWeek.Monday ||
                day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Friday))
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryTueThurs && (day.DayOfWeek == DayOfWeek.Thursday ||
                day.DayOfWeek == DayOfWeek.Tuesday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryWeekday && (day.DayOfWeek != DayOfWeek.Sunday &&
                day.DayOfWeek != DayOfWeek.Saturday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Yearly && evnt.Date.Month == day.Month &&
                evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Monthly && evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Custom && evnt.CustomRecurringFunction != null)
            {
                if (evnt.CustomRecurringFunction(evnt, day))
                    return DayForward(evnt, day);
                return false;
            }

            if (evnt.RecurringFrequency == RecurringFrequencies.None && evnt.Date.Year == day.Year &&
                evnt.Date.Month == day.Month && evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            return false;
        }

        private int NumberOfWeeks(int year, int month)
        {
            return NumberOfWeeks(new DateTime(year, month, DateTime.DaysInMonth(year, month)));
        }

        private int NumberOfWeeks(DateTime date)
        {
            var beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);
            return (int)Math.Truncate(date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        private void CalendarResize(object sender, EventArgs e)
        {
            if (_calendarView == CalendarViews.Day)
                ResizeScrollPanel();
        }        


    }
}