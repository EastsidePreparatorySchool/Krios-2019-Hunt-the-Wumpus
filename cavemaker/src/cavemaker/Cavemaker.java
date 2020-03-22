/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package cavemaker;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Random;

/**
 *
 * @author svernon
 */
public class Cavemaker {

    public static void main(String[] args) {
       ArrayList al = new ArrayList();
       network caves = new network(30);
       
     //  for (int g = 0; g<100; g++){
       
       
       
       caves.connect(1,2,5,6,10);
       caves.connect(2,1,3,10,14);
       caves.connect(3,2,4,14,18);
       caves.connect(4,3,5,18,22);
       caves.connect(5,1,4,6,22);
       caves.connect(6,1,5,7,24);
       caves.connect(7,6,8,9,24);
       caves.connect(8,7,9,26,27);
       caves.connect(9,7,8,10,11);
       caves.connect(10,1,2,9,11);
       
       caves.connect(11,9,10,12,13);
       caves.connect(12,11,13,27,28);
       caves.connect(13,11,12,14,15);
       caves.connect(14,2,3,13,15);
       caves.connect(15,13,14,16,17);
       caves.connect(16,15,17,28,29);
       caves.connect(17,15,16,18,19);
       caves.connect(18,3,4,17,19);
       caves.connect(19,17,18,20,21);
       caves.connect(20,19,21,29,30);
       
       caves.connect(21,19,20,22,23);
       caves.connect(22,4,5,21,23);
       caves.connect(23,21,22,24,25);
       caves.connect(24,6,7,23,25);
       caves.connect(25,23,24,26,30);
       caves.connect(26,8,25,27,30);
       caves.connect(27,8,12,26,28);
       caves.connect(28,12,16,27,29);
       caves.connect(29,16,20,28,30);
       caves.connect(30,20,25,26,29);
       
       //TESTING DISCONNECT/CONNECTING       
//            System.out.println("connections in room 1: "+caves.countCs(1));
//            System.out.println("connections in room 2: "+caves.countCs(2));
//      
//            caves.disconnect(1,2);
//            System.out.println("connections in room 1: "+caves.countCs(1));
//            System.out.println("connections in room 2: "+caves.countCs(2));
//       
//            caves.disconnect(1,5);
//            System.out.println("connections in room 1: "+caves.countCs(1));
//            System.out.println("connections in room 2: "+caves.countCs(2));
//       
//            caves.connect(1,5);
//            caves.connect(1,2);
//            System.out.println("connections in room 1: "+caves.countCs(1));
//            System.out.println("connections in room 2: "+caves.countCs(2));
//            System.out.println("connections in room 5: "+caves.countCs(5));
       
       Random random = new Random(3);

       ArrayList order = new ArrayList(); //a list of the rooms in order we're going to look at
       for(int i = 1; i<31; i++){
           order.add(i);
       }
       //Collections.shuffle(order);
       
       for(int a = 0; a<20; a++){ //20 sweeps through rooms in different orders
       Collections.shuffle(order);
        //System.out.println("order:"+order);
       
       for(int i = 0; i<30; i++){
           int r = (int) order.get(i); //r is ID of room
           int m = random.nextInt(4);
           
           if(caves.countCs(r) > 3){ //r has 4 connections
               int n = 0;
               while(n < 4 && caves.countCs(r)>3){
                   if(caves.countCs(caves.r[r-1].c[m].ID) > 1){ //maybe >2?
                       caves.disconnect(r,caves.r[r-1].c[m].ID);
                   }
                   if(caves.canFind(1) == false){ //we broke the connectivity, must re-connect
                       caves.connect(r,caves.r[r-1].c[m].ID);
                   }
                   m=(m+1)%4;
                   n++;
               }
           }
           
           if(caves.countCs(r) <3){ //r has 1 or 2 connections
               int n = 0;
               while(n < 4 && caves.countCs(r)<3){
                   if(caves.countCs(caves.r[r-1].c[m].ID) < 3){ //maybe >2?
                       caves.connect(r,caves.r[r-1].c[m].ID);
                   }
                   n++;
                   m=(m+1)%4;
               }
           }
           
       }
    }
       
       //looking at output
       int fours = 0;
       al = new ArrayList();
       for(int i = 1; i<31;i++){
          al.add(caves.countCs(i));
          if(caves.countCs(i)==4){
              fours++;
          }
       }
        System.out.println("#connections:"+al);
        System.out.println("max: "+Collections.max(al));
        System.out.println("min: "+Collections.min(al));
        System.out.println("#4 rooms: "+fours);
        System.out.println("fully connected: "+caves.canFind(1));
     
        for (int i = 1; i<31; i++){
            caves.printroom(i);
        }
     
    }
    
}
