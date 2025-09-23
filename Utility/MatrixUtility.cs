 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatrixUtility {

    public string name;
    private int maxRow;
    private int maxCol;
    private int[,] item;


    public MatrixUtility (int newMaxRow,int newMaxCol, string label) {

        maxRow = newMaxRow;
        maxCol = newMaxCol;
        name = label;
        PopulateMatrix ();
        // SimulatePopulateMatrix();

    }

    private void PrintTitle () {

    //    Debug.Log ("->** " + name + " <-");

    }

    

    public void ShowResult () {

     //   PrintTitle ();

        for (int row = 0 ; row < maxRow ; row++) {

            for (int column = 0 ; column < maxCol ; column++) {

              //Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);

            }

        }


    }

    void SimulatePopulateMatrix () {

     //   PrintTitle ();
        item = new int[maxRow,maxCol];

        for (int row = 0 ; row < maxRow ; row++) {

            for (int column = 0 ; column < maxCol ; column++) {

               // item[row,column] = listId[Random.Range (0,listId.Count)];

              // item[row,column] = listId[i];
               // i++;

              //Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);
            }

        }
        List<Vector2> list = new List<Vector2> ();    

        //ShowList (list);
    }

    public void PopulateMatrix () {

        item = new int[maxRow,maxCol];
        PrintTitle ();

        for (int row = 0 ; row < maxRow ; row++) {

            for (int column = 0 ; column < maxCol ; column++) {

                item[row,column] = -1;
              //  Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);
                
            }

        }

    }
    
    public bool IsAreaFree(Vector2 start, int width, int height)
    {
        int startX = (int)start.x;
        int startY = (int)start.y;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int checkRow = startX + x;
                int checkCol = startY + y;

                if (checkRow >= maxRow || checkCol >= maxCol)
                    return false;

                if (item[checkRow, checkCol] != -1)
                    return false;
            }
        }
        return true;
    }

    public bool SetItemFromStart(Vector2 start, int width, int height, int id)
    {
        if (!IsAreaFree(start, width, height))
            return false;

        int startX = (int)start.x;
        int startY = (int)start.y;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                item[startX + x, startY + y] = id;
            }
        }

        return true;
    }



    private void ShowList(List<Vector2> listResult)
    {

        PrintTitle();

        foreach (var item in listResult)
        {
            //     Debug.Log ("Item: " + item);

        }

    }

    public int[,] ShowFullList () {

        return item;
    }

    public void SetItem(List<Vector2> list, int id)
    {

        PrintTitle();

        if (id >= 0)
        {

            foreach (var location in list)
            {

                int row = (int)location.x;
                int column = (int)location.y;
                item[row, column] = id;

                //  Debug.LogWarning ("Row->  #" + row + " | Col->  #" + column + " Item: " + id);


            }


        }
        else { Debug.LogWarning("Do not use id number above 0 Zero"); }

    }

   

    public void ClearItemOnMatrix (int id) {

        PrintTitle ();

        if (id >= 0) {

            for (int row = 0 ; row < maxRow ; row++) {

                for (int column = 0 ; column < maxCol ; column++) {

                    
                    if (item[row,column] == id) {

                        //Debug.LogWarning ("Before");
                        //Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);

                        item[row,column] = -1;

                        //Debug.LogWarning ("After");
                        //Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);

                    }
                }

            }

        }

    }


    public List<Vector2> FindLocationById(int id) {

        List<Vector2> listResult = new List<Vector2> ();

        if(id >= 0) {

            for (int row = 0 ; row < maxRow ; row++) {

                for (int column = 0 ; column < maxCol ; column++) {

                    //int currentId = item[row,column];

                    if (item[row,column] == id) {

                      //Debug.Log ("Row->  #" + row + " | Col->  #" + column + " Item: " + item[row,column]);
                        listResult.Add (new Vector2 (row, column));
                    }
                }
            }
        }

        return listResult;
    }


    public List<Vector2> LookForFreeArea(int number) {

        List<Vector2> listResult = new List<Vector2>();

        if(number == 1) {

            //Debug.LogWarning ("Selected: " + number);

            listResult = LookForAreaHorizontalPriority (number,1);

          //  ShowList (listResult);
            return listResult;
            
        }
      
        if (number == 2) {

            listResult = LookForAreaHorizontalPriority (number,1);

            if (listResult.Count > 1) {
               
                return listResult;

            }
            else {

                listResult = LookForAreaVerticalPriority (number,1);
                if (listResult.Count > 1) {

                    return listResult;

                }
            }
        }


        if (number == 3) {

            listResult = LookForAreaHorizontalPriority (number,1);

            if (listResult.Count > 2) {

                return listResult;

            }

            else {

                listResult = LookForAreaVerticalPriority (number,1);
                if (listResult.Count > 2) {

                    return listResult;

                }
            }

        }

        if (number == 5) {

            listResult = LookForAreaHorizontalPriority (number,1);

            if (listResult.Count > 4) {

                return listResult;

            }
            else {

                listResult = LookForAreaVerticalPriority (number,1);
                if (listResult.Count > 4) {

                    return listResult;

                }
            }

        }

        if (number == 4) {
           // Debug.LogWarning ("Searching 4 ");
            listResult = LookForAreaHorizontalPriority (2,2);

            if (listResult.Count > 3) {

               // ShowList (listResult);
                return listResult;

            }

            else {

                listResult = LookForAreaHorizontalPriority (4,1);
                if (listResult.Count > 3) {

                    // ShowList (listResult);
                    return listResult;

                }
                else {
                    listResult = LookForAreaVerticalPriority (4,1);
                    if (listResult.Count > 3) {

                        // ShowList (listResult);
                        return listResult;

                    }

                }           


            }
        }

        if (number == 6) {

            //Debug.LogWarning ("Searching  6");

            listResult = LookForAreaHorizontalPriority (3,2);

            if (listResult.Count > 5) {                                
               
               // ShowList (listResult);
                return listResult;

            }
            else {

                listResult = LookForAreaVerticalPriority (2,3);
                if (listResult.Count > 5) {                    
                  
                    ShowList (listResult);
                    return listResult;
                }


            }

        }

        if (number == 8) {

            //Debug.LogWarning ("Searching 8 ");
            listResult = LookForAreaHorizontalPriority (4,2);

            if (listResult.Count > 7) {

                //Debug.LogWarning (" LookForAreaHorizontalPriority (4,2)");              
                return listResult;

            }
            else {

                listResult = LookForAreaVerticalPriority (4,2);

                if (listResult.Count > 5) {
                
                  return listResult;
                }


            }
        }



            return listResult;
    }

     public List<Vector2> LookForFreeArea2(int width, int height)
    {
        List<Vector2> result = new List<Vector2>();

        for (int row = 0; row <= maxRow - height; row++)
        {
            for (int col = 0; col <= maxCol - width; col++)
            {
                if (IsAreaFree2(col, row, width, height))
                {
                    for (int dy = 0; dy < height; dy++)
                    {
                        for (int dx = 0; dx < width; dx++)
                        {
                            result.Add(new Vector2(row + dy, col + dx));
                        }
                    }
                    return result;
                }
            }
        }

        return result; // vazio se não encontrar espaço
    }

    public bool IsAreaFree2(int startCol, int startRow, int width, int height)
    {
        for (int dy = 0; dy < height; dy++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                if (item[startRow + dy, startCol + dx] != -1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public List<Vector2> FindLocationById2(int id)
    {
        List<Vector2> positions = new List<Vector2>();

        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                if (item[row, col] == id)
                {
                    positions.Add(new Vector2(row, col));
                }
            }
        }
        return positions;
    }

    List<Vector2> LookForAreaHorizontalPriority (int numberHorizontal, int deep) {

        List<Vector2> listResult = new List<Vector2> ();        
        int colSelect = 0;
        
        //Looking for slot free on Horizontal
        for (int row = 0 ; row < maxRow ; row++) {

            for (int column = 0 ; column < maxCol ; column++) {

                //Debug.Log ("FOR column: " + column);

                if (item[row,column] == -1) {

                    colSelect = column;
                    //Debug.Log ("------ First Free: -> Horizontal <-");
                    //Debug.Log ("Row: " + row + " Col: " + column + " | " + item[row,column]);

                    if (maxCol - column >= numberHorizontal) {

                        //Debug.Log ("-> There is HOPE _H_");

                        for (int i = colSelect ; i < maxCol ; i++) {

                            //Debug.Log ("Long Search");

                          
                            if (VerticalValidation (row,i,deep)) {


                                for (int add = 0 ; add < deep ; add++) {

                                    if (row + add < maxRow) {

                                        if (item[row + add,i] == -1) {

                                            listResult.Add (new Vector2(row + add,i));
                                            int w = row + add;
                                            //Debug.Log ("-> ADD Row: " + w + " Col: " + i + " | " + item[w,i]);


                                        }
                                        // IN TESTE
                                        else {

                                            listResult.Clear ();


                                        }
                                        //
                                    }

                                    if (listResult.Count == numberHorizontal * deep) {

                                        
                                      //Debug.Log ("Completed Horizontal searching");
                                        
                                        /*
                                        foreach (var item in listResult) {

                                          Debug.Log ("Item: " + item);

                                        }                                        
                                        */
                                        return listResult;
                                    }

                                    //Max element                                
                                      if (i == maxCol ) {                             
                                        
                                      //Debug.Log ("Colum Limit");
                                        listResult.Clear ();

                                    }


                                }

                            }
                                            
                           
                            
                            
                            else {

                                //Debug.Log ("There is a GAP in Row: " + row + " Col: " + i + " | " + item[row,i]);

                                listResult.Clear ();

                                //get the whole column test 
                                column = i;

                                //Debug.Log ("-> STOPPED in Row: " + row + " Col: " + column + " | " + item[row,column]);

                                if (maxCol - column <= numberHorizontal) {

                                    //Debug.Log ("## THERE IS NO HOPE");
                                    column = maxCol;

                                }

                                break;
                            }
                        }
                    }
                }
                //End Hope
               // else { Debug.Log ("Drop to next row"); }
                // In Test
                listResult.Clear ();
                //
            }
        }


        return listResult;
    }

    private List<Vector2> LookForAreaVerticalPriority (int numberVertical,int deep) {

       

        List<Vector2> listResult = new List<Vector2> ();
        
        int rowSelect = 0;

        //Looking for slot free on Vertical
        for (int column = 0 ; column < maxCol ; column++) {

            for (int row = 0 ; row < maxRow ; row++) {


                if (item[row,column] == -1) {

                    rowSelect = row;

                    //Debug.Log ("------ First Free --------->        Vertical");
                    //Debug.Log ("Row: " + row + " Col: " + column + " | " + item[row,column]);

                    if (maxRow - row >= numberVertical) {

                        // Debug.Log ("## There is HOPE");


                        for (int i = rowSelect ; i < maxRow ; i++) {

                            //   Debug.Log ("Long Search");

                            if (HorizontalValidation (i,column,deep)) {

                                for (int add = 0 ; add < deep ; add++) {

                                    if (column + add < maxCol) {

                                        if (item[i,column + add] == -1) {

                                            listResult.Add (new Vector2 (i,column + add));

                                            int w = column + add;
                                            //Debug.Log ("-> ADD Row: " + i + " Col: " + w + " | " + item[i,w]);

                                        }
                                        else {

                                           listResult.Clear ();
                                           

                                        }
                                    }                                



                                }

                                if (listResult.Count == numberVertical * deep) {

                                    /*
                                    Debug.Log ("Completed Vertical searching");
                                    
                                    foreach (var item in listResult) {
                                        Debug.Log ("Item: " + item);

                                    }
                                    */

                                    return listResult;
                                }

                                //Max element

                                //It was this way=>
                                //if (i == maxRow - 1) {

                                    if (i == maxRow )  {

                                    // Debug.Log ("Colum Limit");
                                    listResult.Clear ();
                                }


                            }
                            else {

                                //Debug.Log ("There is a GAP in Column: " + i + " Col: " + column + " | " + item[i,column]);
                                listResult.Clear ();

                                //get the whole column test 
                                row = i;
                                //Debug.Log ("-> STOPPED in Column: " + row + " Col: " + column + " | " + item[row,column]);

                                if (maxRow - row <= numberVertical) {

                                    //Debug.Log ("## THERE IS NO HOPE");
                                    row = maxRow;

                                }

                                break;

                            }
                        }
                    }//End Hope
                    else {
                        //Debug.Log ("Drop to next Column");
                        listResult.Clear ();

                    }

                }

            }

        }
       

        return listResult;

    }


    private bool VerticalValidation (int currentRow,int currentColumn,int deepVertical) {
       
        bool result = true;

        if (deepVertical == 1) {

            return result;
        }

        //its was my last added 
        
        if (deepVertical + currentRow > maxRow) {
            
            return false;
        }
        

        int limit = deepVertical + currentRow;
    

        for (int row = currentRow ; row < limit ; row++) {
                      

            result = result && (item[row,currentColumn] == -1);
         


        }

        return result;
    }

    private bool HorizontalValidation (int currentRow,int currentColumn,int deepHorizontal) {

        bool result = true;

        if (deepHorizontal == 1) {

            return result;
        }

        //its was my last added 
        if (deepHorizontal + currentColumn > maxCol) {

            return false;
        }


        int limit = deepHorizontal + currentColumn;


        for (int column = currentColumn ; column < limit ; column++) {

            result = result && (item[currentRow,column] == -1);
     

        }

        return result;
    }

    
}
