﻿using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarCreator
{
    public static class CategoryHelper
    {
        public static IEnumerable<Category> GetCategories(BodyType bodyType)
        {
            return PartnerCategoryMap
                .Select(a => a.Value)
                .Where(category => category.IsCompatibleCategory(bodyType))
                .ToList();
        }

        public static readonly Dictionary<string, Category> PartnerCategoryMap = new Dictionary<string, Category>
        {
            { "faceshape", Category.FaceShape },
            { "eyeshape", Category.EyeShape },
            { "eye", Category.EyeColor },
            { "eyebrows", Category.EyebrowStyle },
            { "noseshape", Category.NoseShape },
            { "lipshape", Category.LipShape },
            { "beard", Category.BeardStyle },
            { "hair", Category.HairStyle },
            { "outfit", Category.Outfit },
            { "shirt", Category.Shirt },
            { "glasses", Category.Glasses },
            { "facemask", Category.FaceMask },
            { "facewear", Category.Facewear },
            { "headwear", Category.Headwear },
            { "hairColor", Category.HairColor },
            { "eyebrowColor", Category.EyebrowColor },
            { "beardColor", Category.BeardColor },
            { "bottom", Category.Bottom },
            { "top", Category.Top },
            { "footwear", Category.Footwear }
        };

        public static bool IsOutfitAsset(this Category category)
        {
            switch (category)
            {
                case Category.Outfit:
                case Category.Shirt:
                case Category.Bottom:
                case Category.Top:
                case Category.Footwear:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool IsFaceAsset(this Category category)
        {
            switch (category)
            {
                case Category.FaceShape:
                case Category.EyeShape:
                case Category.EyeColor:
                case Category.EyebrowStyle:
                case Category.NoseShape:
                case Category.LipShape:
                case Category.BeardStyle:
                    return true;
                default:
                    return false;
            }
        }
        
        private static bool IsCompatibleCategory(this Category category, BodyType bodyType)
        {
            // Filter asset type based on body type.
            if (bodyType == BodyType.FullBody)
            {
                return category != Category.Shirt;
            }
            return category != Category.Outfit;
        }
        
        public static bool IsOptionalAsset(this Category category)
        {
            switch (category)
            {
                case Category.Top:
                case Category.Bottom:
                case Category.Footwear:
                case Category.Outfit:
                case Category.Shirt:
                case Category.EyebrowStyle:
                    return false;
                default:
                    return !category.IsColorAsset();
            }
        }
        
        public static bool IsColorAsset(this Category category)
        {
            switch (category)
            {
                case Category.EyeColor:
                case Category.BeardColor:
                case Category.EyebrowColor:
                case Category.HairColor:
                case Category.SkinColor:
                    return true;
                default:
                    return false;
            }
        }
    }
}
