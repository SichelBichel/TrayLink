namespace TrayLink
{
    /// <summary>What a card needs from the list that hosts it.</summary>
    public interface ILinkItemHost
    {
        void ExecuteAction(ActionConfig config);
        void EditAction(ActionConfig config);
        void DeleteAction(ActionConfig config);
        void MoveAction(ActionConfig config, int delta);

        /// <summary>False while the search filter is active (order of a filtered list is ambiguous).</summary>
        bool CanReorder { get; }

        void OnCardDragFinished(LinkItem card, bool dropped);
    }

    public partial class LinkItem : UserControl
    {
        public const string DragFormat = "TrayLink.LinkItem";

        private readonly ILinkItemHost _host;
        private readonly Color _normalBackColor;
        private bool _mouseDown;
        private Point _mouseDownScreenPos;

        public ActionConfig Config { get; }

        public LinkItem(ActionConfig config, int position, ILinkItemHost host)
        {
            InitializeComponent();
            Config = config;
            _host = host;
            _normalBackColor = BackColor;

            labelPos.Text = "#" + position;
            labelType.Text = config.ActionType.ToUpperInvariant();
            labelName.Text = config.ActionName;

            var tip = $"{config.ActionName}\n{config.PathOrUrl}";
            toolTip.SetToolTip(this, tip);
            toolTip.SetToolTip(labelName, tip);
            toolTip.SetToolTip(labelType, tip);

            // Drag + drop: grab the card anywhere except on the buttons.
            foreach (var c in new Control[] { this, labelName, labelType, labelPos })
            {
                c.MouseDown += Card_MouseDown;
                c.MouseMove += Card_MouseMove;
                c.MouseUp += Card_MouseUp;
            }

            UpdateReorderState();
        }

        /// <summary>Shows the move cursor and enables the arrow buttons only when reordering is possible.</summary>
        public void UpdateReorderState()
        {
            bool canReorder = _host.CanReorder;
            Cursor = canReorder ? Cursors.SizeAll : Cursors.Default;
            buttonUp.Enabled = canReorder;
            buttonDown.Enabled = canReorder;
        }

        private void Card_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            _mouseDown = true;
            _mouseDownScreenPos = Control.MousePosition;
        }

        private void Card_MouseUp(object? sender, MouseEventArgs e) => _mouseDown = false;

        private void Card_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_mouseDown || e.Button != MouseButtons.Left || !_host.CanReorder)
                return;

            var now = Control.MousePosition;
            var dragSize = SystemInformation.DragSize;
            if (Math.Abs(now.X - _mouseDownScreenPos.X) < dragSize.Width &&
                Math.Abs(now.Y - _mouseDownScreenPos.Y) < dragSize.Height)
                return;

            _mouseDown = false;
            BeginCardDrag();
        }

        private void BeginCardDrag()
        {
            DragDropEffects result = DragDropEffects.None;
            BackColor = Theme.DragHighlight;
            Invalidate();
            try
            {
                var data = new DataObject();
                data.SetData(DragFormat, this);
                result = DoDragDrop(data, DragDropEffects.Move);
            }
            finally
            {
                if (!IsDisposed)
                    BackColor = _normalBackColor;
            }

            _host.OnCardDragFinished(this, result == DragDropEffects.Move);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Theme.PaintCardFrame(e.Graphics, ClientRectangle, highlight: BackColor == Theme.DragHighlight);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        private void inputExecute(object sender, EventArgs e) => _host.ExecuteAction(Config);

        private void inputEditEntry(object sender, EventArgs e) => _host.EditAction(Config);

        private void inputDeleteEntry(object sender, EventArgs e) => _host.DeleteAction(Config);

        private void ClickMoveUp(object sender, EventArgs e) => _host.MoveAction(Config, -1);

        private void clickMoveDown(object sender, EventArgs e) => _host.MoveAction(Config, +1);
    }
}
