using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ExampleMod.Content.Items
{
	internal class ExampleAdvancedTooltipDrawingItem : ModItem
	{
		public override string Texture => "Terraria/Images/Item_3617";

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
		}

		private Vector2 boxSize; // Stores the size of our tooltip box
		private const int paddingForBox = 10; // Extra size for our box so the tooltips aren't directly on the edge

		public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
			// You can offset the entire tooltip by changing x and y
			// You can actually have the entire tooltip draw somewhere else, x and y is where the tooltip starts drawing

			// Draw a magic box for this tooltip

			var texts = lines.Select(z => z.Text); // Get the texts of the tooltips

			// Calculate our width for the box, which will be the width of the longest text, plus some padding. This code takes into account Snippets and character widths.
			int widthForBox = texts.Max(t => (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, t, Vector2.One).X) + paddingForBox * 2;
			// Calculate our height for the box, which will be the sum of the text heights, plus some padding
			int heightForBox = (int)texts.ToList().Sum(z => FontAssets.MouseText.Value.MeasureString(z).Y) + paddingForBox * 2;

			// Set our boxSize to our calculated size so we can use this elsewhere too
			boxSize = new Vector2(widthForBox, heightForBox);

			// We will start drawing the box slightly offset to accommodate for padding
			Vector2 drawPosForBox = new Vector2(x - paddingForBox, y - paddingForBox);
			Rectangle drawRectForBox = new Rectangle((int)drawPosForBox.X, (int)drawPosForBox.Y, widthForBox, heightForBox);

			// Draw the magic box
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawRectForBox, Main.MouseTextColorReal);

			return true;
		}

		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
			if (line.OneDropLogo) {
				// You are not allowed to change this line. Modders should use ModifyTooltips instead
				return true;
			}

			// Let's draw the item name centered so it's in the middle, and let's add a form of separator
			string sepText = "-----"; // This is our separator, which will go between the item name and the rest
			float sepHeight = line.Font.MeasureString(sepText).Y; // Height of our separator

			// If our line text equals our item name, this is our tooltip line for the item name
			// if (line.text == item.HoverName)
			// What is more accurate to check is the layer name and mod
			if (line.Name == "ItemName" && line.Mod == "Terraria") {
				// We check for Terraria so we modify the vanilla tooltip and not a modded one
				// This could be important, in case some mod does a lot of custom work and removes the standard tooltip
				// For tooltip layers, check the documentation for TooltipLine

				// Our offset is half the width of our box, minus the padding of one side
				float boxOffset = boxSize.X / 2 - paddingForBox;

				// The X coordinate where we draw is where the line would draw, plus the box offset,
				// which would place the START of the string at the center, so we subtract half of the line width to center it completely
				float drawX = line.X + boxOffset - line.Font.MeasureString(sepText).X / 2;
				float drawY = line.Y + sepHeight / 2;

				// Note how our line object has many properties we can use for drawing
				// Here we draw the separator. PostDraw would also work for this.
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					line.Font,
					sepText,
					new Vector2(drawX, drawY),
					line.Color,
					line.Rotation,
					line.Origin,
					line.BaseScale,
					line.MaxWidth,
					line.Spread
				);

				// Here we do the same thing as we did for drawX, which will center our ItemName tooltip
				line.X += (int)boxOffset - (int)line.Font.MeasureString(line.Text).X / 2;
				// yOffset affects the offset that is added every next line, so this will cause the line to come after the separator to be drawn slightly lower
				yOffset = (int)sepHeight / 4;
			}
			else {
				// Reset the offset for other lines
				yOffset = 0;
			}

			return true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.Register();
		}
	}
}
