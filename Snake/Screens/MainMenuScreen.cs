using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Screens
{
    public class MainMenuScreen : IScreen
    {
        /// <summary>
        /// ScreenManager which handles this screen.
        /// </summary>
        private ScreenManager screenManager;
        /// <summary>
        /// Texture displayed behind menu items.
        /// </summary>
        private Texture2D backgroundTexture;
        /// <summary>
        /// List of menu items that are displayed on this menu.
        /// </summary>
        private List<MainMenuItem> menuItems;
        /// <summary>
        /// Padding between items on the menu.
        /// </summary>
        private int padding = 10;
        /// <summary>
        /// Position in list of the currently selected item.
        /// </summary>
        private int selected;
        /// <summary>
        /// Font menu text is displayed in.
        /// </summary>
        private SpriteFont font;
        /// <summary>
        /// The font the menu text is displayed in when it has been selected/moused over.
        /// </summary>
        private SpriteFont selectedFont;
        /// <summary>
        /// The colour the menu text is displayed in.
        /// </summary>
        private Color textColor;

        /// <summary>
        /// The name identifier of screen,
        /// </summary>
        public string ScreenName { get; private set; }
        /// <summary>
        /// A texture to go behind the button text.
        /// </summary>
        public Texture2D ButtonTexture { get; set; }
        /// <summary>
        /// Font when the user has the current item selected, may be bolded or underlined or some such.
        /// </summary>
        public SpriteFont SelectedFont
        {
            get { return selectedFont; }
            //if the selected font is changed after being initialised the rectangles need to be changed because the selected font might be different size.
            set { selectedFont = value; if (menuItems != null) { CalcRectangles(); }; }
        }
        /// <summary>
        /// Color when the user has the current item selected.
        /// </summary>
        public Color? SelectedColor { get; set; }
        /// <summary>
        /// Whether the current screen is active.
        /// </summary>
        public bool Active { get; set; }


        /// <summary>
        /// Create a MainMenuScreen which can be initialised with a list of MainMenuItems.
        /// </summary>
        /// <param name="screenManager">The screen manager that handles this screen.</param>
        /// <param name="name">The name identifier for this screen.</param>
        public MainMenuScreen(ScreenManager screenManager, string name)
        {
            this.screenManager = screenManager;
            this.ScreenName = name;
            Active = false;
            SelectedFont = null;
            SelectedColor = null;
        }

        /// <summary>
        /// Create a MainMenuScreen which can be initialised with a list of MainMenuItems.
        /// </summary>
        /// <param name="screenManager">The screen manager that handles this screen.</param>
        /// <param name="name">The name identifier for this screen.</param>
        /// <param name="padding">The padding between items on the menu.</param>
        public MainMenuScreen(ScreenManager screenManager, string name, int padding) : this(screenManager, name)  {
            this.padding = padding;
        }

        /// <summary>
        /// Initialise the Menu with the loaded content of textures, fonts and menu items.
        /// </summary>
        /// <param name="backgroundTexture">A background image to be drawn behind the menu items. Can be null.</param>
        /// <param name="menuItems">List of MainMenuItems that will make up options on the menu.</param>
        /// <param name="font">Font used for writing text of regular menu items.</param>
        /// <param name="textColor">Color to draw the text.</param>
        public void Initialize(Texture2D backgroundTexture, List<MainMenuItem> menuItems, SpriteFont font, Color textColor)
        {
            if (menuItems == null || menuItems.Count == 0)
            {
                throw new ArgumentNullException("Menu \"" + ScreenName + "\" requires some items.");
            }
            if (font == null)
            {
                throw new ArgumentNullException("Font is null in " + ScreenName);
            }
            if (textColor == null)
            {
                textColor = Color.Transparent;
            }

            this.backgroundTexture = backgroundTexture;
            this.menuItems = menuItems;
            this.font = font;
            this.textColor = textColor;
            this.selected = 0;
            CalcRectangles();
        }

        /// <summary>
        /// Called each frame to update the menu.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="inputState">The state of the input controls.</param>
        public void Update(GameTime gameTime, InputState inputState)
        {
            //The original selected item, if after checking input it has changed we will reupdate where everything is located.
            int initialSelectedBeforeUpdate = selected;

            UpdateKeyboardInput(inputState);
            UpdateMouseInput(inputState);

            //if the current selected item has changed update the rectangles. Only need to do this if we have a diferent selected font.
            if (initialSelectedBeforeUpdate != selected && selectedFont!= null)
            {
                CalcRectangles();
            }

            UpdateUserClickingItem(inputState);
        }

        private void UpdateUserClickingItem(InputState inputState)
        {
            if (inputState.KeyWasUpNowDown(Keys.Enter))
            {
                ClickSelectedItem();
            }
            if (inputState.OldMouseState.LeftButton == ButtonState.Released && inputState.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                //if when mouse is clicked it is over the current selected button.
                if (menuItems.ElementAt(selected).Location.Contains(inputState.CurrentMouseState.Position))
                {
                    ClickSelectedItem();
                }
            }
        }

        private void ClickSelectedItem()
        {
            menuItems.ElementAt(selected).ItemClicked();
        }

        private void UpdateMouseInput(InputState inputState)
        {
            if (inputState.HasMouseMoved())
            {
                Point p = inputState.CurrentMouseState.Position;
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (menuItems.ElementAt(i).Location.Contains(p))
                    {
                        selected = i;
                        break;
                    }
                }
            }
        }

        private void UpdateKeyboardInput(InputState inputState)
        {
            if (inputState.KeyWasUpNowDown(Keys.Up))
            {
                if (selected == 0)
                {
                    selected = menuItems.Count - 1;
                }
                else
                {
                    selected--;
                }
            }
            if (inputState.KeyWasUpNowDown(Keys.Down))
            {
                selected++;
                selected %= menuItems.Count;
            }
        }

        
        /// <summary>
        /// Called every frame to draw the menu.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">Spritebatch to use to draw.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (backgroundTexture != null)
            {
                //difference between the image size
                //Vector2 difference = new Vector2(backgroundTexture.Width, backgroundTexture.Height);
                spriteBatch.Draw(backgroundTexture, new Rectangle(0,0,screenManager.ScreenWidth,screenManager.ScreenHeight), Color.White);
            }
            for (int i = 0; i < menuItems.Count; i++ )
            {
                if (ButtonTexture != null)
                {
                    spriteBatch.Draw(ButtonTexture, menuItems.ElementAt(i).Location, Color.Transparent);
                }
                if (i == selected)
                {
                    Color c = SelectedColor ?? textColor;
                    SpriteFont f = SelectedFont ?? font;
                    spriteBatch.DrawString(f, menuItems.ElementAt(i).Text, menuItems.ElementAt(i).Location.Location.ToVector2() , c);
                }
                else
                {
                    spriteBatch.DrawString(font, menuItems.ElementAt(i).Text, menuItems.ElementAt(i).Location.Location.ToVector2(), textColor);
                }
            }
        }


        

        /// <summary>
        /// Calculate a vector of the size of the string displayed in the passed in font.
        /// </summary>
        /// <param name="font">The font used to display.</param>
        /// <param name="s">The string that would be displayed in the font.</param>
        /// <returns>A vector 2 of the height and width the string would be in the specified font.</returns>
        private Vector2 CalcStringSize(SpriteFont font, string s)
        {
            return font.MeasureString(s);
        }
        
        /// <summary>
        /// Calculate a rectangle at size and position that the passed in string in the menuItems will be displayed at.
        /// </summary>
        /// <param name="s">The string to be displayed.</param>
        /// <param name="selected">whether the current item is selected.</param>
        /// <param name="yPosition">the y position of the rectangle.</param>
        /// <returns></returns>
        private Rectangle CalcRectanglePosition(string s, bool selected, int yPosition)
        {
            Rectangle rec;
            Vector2 vec;
            if (selected && (SelectedFont != null))
            {
                    vec = CalcStringSize(SelectedFont, s);
            }
            else {
                vec = CalcStringSize(font, s);
            }
            rec.Width = (int)vec.X;
            rec.Height = (int)vec.Y;
            rec.X = (screenManager.ScreenWidth / 2) - (rec.Width / 2);
            rec.Y = yPosition;
            return rec;
        }

        /// <summary>
        /// Calculate the rectangles for each of the menu items that store the size and position of the text they will display.
        /// </summary>
        private void CalcRectangles()
        {
            //need to know the height of all the items so we can know where to begin placing our list of items.
            int totalHeight = CalcTotalHeightOfMenuItems();
            int currentY = (screenManager.ScreenHeight / 2) - (totalHeight / 2);
            for(int i =0; i < menuItems.Count; i++)
            {
                //create the rectangle that is the size and position of the string at the same position in textBoxes as it is in menuItems.
               menuItems[i].Location = CalcRectanglePosition(menuItems.ElementAt(i).Text, i == selected, currentY);
                //add the height of current item and padding to begin creating next rectangle at position enough below.
                currentY += menuItems[i].Location.Height + padding;
            }
        }

        /// <summary>
        /// Calculate the total height the list of menu items will need to be displayed.
        /// </summary>
        /// <returns>The height the menu items will use in total.</returns>
        private int CalcTotalHeightOfMenuItems()
        {
            int count = 0;
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selected && SelectedFont != null)
                {
                    count += (int)CalcStringSize(SelectedFont, menuItems.ElementAt(i).Text).Y;
                }
                else
                {
                    count += (int)CalcStringSize(font, menuItems.ElementAt(i).Text).Y;
                }
            }
            count += (menuItems.Count - 1) * padding;
            return count;
        }
        
    }
}
