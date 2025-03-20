using Chess_game.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public static class PromotionLists_Factory
    {

        public static Dictionary<ushort, List<ushort>> PromotionIDList_To_PromotionChessPiecesIDList = new();

        static PromotionLists_Factory()
        {
            List<ushort> pieceList = new()
            {
                 4, 5, 6, 7
            };

            pieceList = pieceList.OrderByDescending(Pieces_Factory.Get_PieceValuability_UsingID).ToList();

            PromotionIDList_To_PromotionChessPiecesIDList.Add(1, pieceList);
        }





        public static IReadOnlyCollection<ushort> Get_PromotionList_ByID(ushort id)
        {
            List<ushort> result = new ();

            if (PromotionIDList_To_PromotionChessPiecesIDList.TryGetValue(id, out var list))
            {

                foreach(var piece in list)
                {
                    if (Pieces_Factory.Contains_Piece_WithID(piece))
                    {
                        result.Add(piece);
                    }
                }
            }

            return result;
        } 

    }
}
