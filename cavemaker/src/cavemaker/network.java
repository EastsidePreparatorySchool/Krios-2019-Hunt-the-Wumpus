/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cavemaker;

import java.util.Arrays;
import java.util.Collections;

/**
 *
 * @author svernon
 */
public class network {
    
    room[] r;
    
    public network(int n){
        r = new room[n];
        for (int i = 0; i<n; i++){
          r[i] = new room(i+1); // room "names" start at 1 and count up
       }
    }
    
    //connect one to all of its connections (use this one to set up map)
    public void connect(int o, int a, int b, int c, int d){
        r[o-1].connect(r[a-1],r[b-1],r[c-1],r[d-1]);
    }
    
    //connect to one connection (use this one to edit an existing map)
    public void connect(int o, int t){
        //find location of t (tl) in o connections
        int tl = 0;
        while(r[o-1].c[tl].ID != t){
            tl++;
            if(tl>3){
               // System.out.println("failure to find "+t+" from room "+o);
                break;
            }
        }
        r[o-1].state[tl] = true;
        //repeat for opposite side of the connection
        int ol = 0;
        while(r[t-1].c[ol].ID != o){
            ol++;
            if(ol>3){
                System.out.println("failure to find "+o+" from room "+t);
                break;
            }
        }
        r[t-1].state[ol] = true;
    }
    
    public void printroom(int a){
        r[a-1].printroom();
    }
    
    public void disconnect(int o, int t){
        //find location of t (tl) in o connections
        //System.out.println("disconnecting");
        int tl = 0;
        while(r[o-1].c[tl].ID != t){
            tl++;
            if(tl>3){
                System.out.println("failure to find "+t+" from room "+o);
                break;
            }
        }
        //System.out.println("i found "+t+" at "+tl+" in "+o);
        r[o-1].state[tl] = false;
        //repeat for opposite side of the connection
        int ol = 0;
        while(r[t-1].c[ol].ID != o){
            ol++;
            if(ol>3){
                System.out.println("failure to find "+o+" from room "+t);
                break;
            }
        }
        r[t-1].state[ol] = false;
        //System.out.println("disconnected "+o+" and "+t);
    }
    
    public int countCs(int o){
        int n = 4;
        for(int i = 0; i<4; i++){
            if (r[o-1].state[i] == false){
                n--;
            }
        }
        return n;
    }
    
    //from room #o, can we find all other rooms?
    boolean canFind(int o){
        boolean result = false;
        int[] found = new int[30];
        int c0 = 0;
        found = canFind2(o, found);
        //System.out.println(Arrays.toString(found));
        int i = 0;
        int j = 0;
        
        while(1==1){
            j = i;
            while(found[i]!=1){
                i = (i+1)%30;
                if(i == j){ //went through whole array, no new connections
                    //check for all 0s? if all 0s return false
                    for(int n = 0; n<30; n++){
                        if(found[n] == 0){
                            c0++;
                        }
                    }
                    if(c0>0){
                        return false; // we explored all connections to o but, but still didn't reach some rooms
                    } else {
                        return true;
                    }
                }
            } //i found a 1 at [i]
            found = canFind2((i+1), found); //update found array
            //System.out.println(Arrays.toString(found));
        }
    }
    
    int[] canFind2(int t, int[] ar){
        if(ar[t-1]==0){
            ar[t-1] = 2;
        } else {
            ar[t-1]++;
        }
        for(int i = 0; i<countCs(t); i++) { //for every [open] connection room has
            if(r[t-1].state[i] == true){
                if(ar[r[t-1].c[i].ID-1] == 0){ //this is the first connection we've seen to that room!
                    ar[r[t-1].c[i].ID-1] ++; //add a 1 so we know to explore later
                }
            } else {
                //System.out.println("theres a disconnect between "+(t)+" and "+r[t-1].c[i].ID);
            }
        }
        return ar;
    }
    
}
