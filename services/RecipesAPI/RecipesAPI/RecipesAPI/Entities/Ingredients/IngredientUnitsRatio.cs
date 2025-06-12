using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    [Table("ingredient_units_ratios")]
    public class IngredientUnitsRatio
    {
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Column("unit_one_id")]
        public Guid UnitOneId { get; set; }

        [Column("unit_two_id")]
        public Guid UnitTwoId { get; set; }

        [Column("one_to_two_ratio")]
        public double OneToTwoRatio { get; set; }

        public virtual Ingredient Ingredient { get; set; }
        public virtual Unit UnitOne { get; set; }
        public virtual Unit UnitTwo { get; set; }
    }
}
