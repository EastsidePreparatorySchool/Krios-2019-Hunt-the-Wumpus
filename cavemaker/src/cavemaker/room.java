/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cavemaker;

/**
 *
 * @author svernon
 */
public class room {
    
    int ID = 0;
    
    boolean[] state; //whether edges are open/blocked
    room[] c; //connections (the rooms its connected to)
    
    public void printroom(){
        System.out.println(ID+" Room connections:");
        if(state[0] == true){
            System.out.println(" [0] ->"+c[0].ID);
        }
        if(state[1] == true){
            System.out.println(" [1] ->"+c[1].ID);
        }
        if(state[2] == true){
            System.out.println(" [2] ->"+c[2].ID);
        }
        if(state[3] == true){
            System.out.println(" [3] ->"+c[3].ID);
        }

    }
    
    //connecting the map originally (setting up)
    public void connect(room w, room x, room y, room z){
        c[0] = w;
        c[1] = x;
        c[2] = y;
        c[3] = z;
        for (int i = 0; i<4; i++){
            state[i] = true;
        }
    }
    
    public room(int i){
        this.ID = i;
        state = new boolean[4];
        c = new room[4];
    }
    
}
