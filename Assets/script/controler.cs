using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pd;


namespace pd
{
    public interface ISceneController
    {
        void loadResources();
    }

    public interface IUserAction
    {
        void reset();
        void moveBoat();
        void movePD(pd_controller controling);

    }


    public class Director : System.Object
    {
        private static Director _instance;
        public ISceneController currentSceneController { get; set; }

        public static Director getInstance()
        {
            if (_instance == null)
            {
                _instance = new Director();
                return _instance;
            }
            else return _instance;
        }
    }

    public class pd_controller
    {
        GameObject pd;
        control_moving move;
        click_gui cg;
        bool type;
        public pd_controller(bool pORd)
        {
            if(pORd)
            {
                pd = Object.Instantiate(Resources.Load("perfab/p", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
                type = true;
            }
            else
            {
                pd = Object.Instantiate(Resources.Load("perfab/d", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
                type = false;
            }
            move = pd.AddComponent(typeof(control_moving)) as control_moving;
            cg = pd.AddComponent(typeof(click_gui)) as click_gui;
            cg.set_controling(this);
            
        }

        public bool get_type()
        {
            return type;
        }

        public void set_pos(Vector3 pos)
        {
            move.set(pos);
        }

        public void set_coast(coast_controller side)
        {
            pd.transform.parent = null;
        }

        public void set_boat(boat_controller boat)
        {
            pd.transform.SetParent(boat.get_boat().transform);
            Debug.Log(pd.transform.parent);
        }

        public void set_name(string name)
        {
            pd.name = name;
        }

        public string get_name()
        {
            return pd.name;
        }

        public void reset()
        {
            pd.transform.parent = null;
        }

        
    }

    public class coast_controller
    {
        GameObject coast;
        pd_controller[] pds; 
        bool side; // true right, false left


        public coast_controller(bool _side)
        {
            pds = new pd_controller[6];
            if (_side)
            {
                coast = Object.Instantiate(Resources.Load("perfab/coast", typeof(GameObject)) , new Vector3(9, 1, 0), Quaternion.identity, null) as GameObject;
            }
            else
            {
                coast = Object.Instantiate(Resources.Load("perfab/coast", typeof(GameObject)), new Vector3(-9, 1, 0), Quaternion.identity, null) as GameObject;
            }
            side = _side;
        }

        public Vector3 GetEmptyPos()
        {
            int index = -1;
            for(int i = 0; i < pds.Length; i++)
            {
                if(pds[i] == null)
                {
                    index = i;
                    break;
                }
            }
            if(index == -1)
            {
                return Vector3.zero;
            }
            if(side)
            {
                return new Vector3(6.5F + index, 2.5F, 0);
            }
            else
            {
                return new Vector3(-6.5F - index, 2.5F, 0);
            }
        }

        public GameObject get_coast()
        {
            return coast;
        }

        public void insert(pd_controller pOrd)
        {
            for(int i = 0; i < pds.Length; i++)
            {
                if(pds[i] == null)
                {
                    pds[i] = pOrd;
                    return;
                }
            }
            return;
        }

        public void remove(pd_controller pORd)
        {
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_name() == pORd.get_name())
                {
                    pds[i] = null;
                    return;
                }
            }
        }

        public bool is_on_coast(pd_controller pORd)
        {
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_name() == pORd.get_name())
                {
                    return true;
                }
            }
            return false;
        }

        public int get_pcount()
        {
            int ans = 0;
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_type())
                    ans++;
            }
            return ans;
        }

        public int get_dcount()
        {
            int ans = 0;
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && !pds[i].get_type())
                    ans++;
            }
            return ans;
        }

        public void reset()
        {
            for (int i = 0; i < pds.Length; i++)
                pds[i] = null;

            
        }

    }

    public class boat_controller
    {
        GameObject boat;
        control_moving move;
        bool goto_side;
        pd_controller[] pds = new pd_controller[2];
        click_gui cg;

        public boat_controller()
        {
            goto_side = true;
            boat = Object.Instantiate(Resources.Load("perfab/boat", typeof(GameObject)), new Vector3(4.5F, 1, 0), Quaternion.identity, null) as GameObject;
            boat.name = "boat";
            move = boat.AddComponent(typeof(control_moving)) as control_moving;
            cg = boat.AddComponent(typeof(click_gui)) as click_gui;
        }

        public void Move()
        {
            if(goto_side)
            {
                move.set(new Vector3(-4.5F, 1, 0));
                goto_side = false;
            }
            else
            {
                move.set(new Vector3(4.5F, 1, 0));
                goto_side  = true;
            }
        }

        public bool empty()
        {
            for(int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null)
                    return false;
            }
            return true;
        }

        public Vector3 GetEmptyPos()
        {
            int index = -1;
            for(int i = 0; i < pds.Length; i++)
            {
                if(pds[i] == null)
                {
                    index = i;
                    break;
                }
            }
            
            if (index == -1)
            {
                return Vector3.zero;
            }
           
            if(goto_side)
            {
                return new Vector3(4.5F + index, 1.6F, 0);
            }
            else
            {
                return new Vector3(-4.5F - index, 1.6F, 0);
            }
        }

        public GameObject get_boat()
        {
            return boat;
        }

        public bool get_side()
        {
            return goto_side;
        }

        public void insert(pd_controller pORd)
        {
            for(int i = 0; i < pds.Length; i++)
            {
                if(pds[i] == null)
                {
                    pds[i] = pORd;
                    return;
                }
            }
        }

        public void remove(pd_controller pORd)
        {
            for(int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_name() == pORd.get_name())
                {
                    pds[i] = null;
                    return;
                }
            }
        }

        public bool is_on_boat(pd_controller pORd)
        {
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_name() == pORd.get_name())
                {
                    return true;
                }
            }
            return false;
        }

        public bool is_full()
        {
            for(int i = 0; i < pds.Length; i++)
            {
                if (pds[i] == null)
                    return false;
            }
            return true;
        }

        public int get_pcount()
        {
            int ans = 0;
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && pds[i].get_type())
                    ans++;
            }
            return ans;
        }

        public int get_dcount()
        {
            int ans = 0;
            for (int i = 0; i < pds.Length; i++)
            {
                if (pds[i] != null && !pds[i].get_type())
                    ans++;
            }
            return ans;
        }

        public void reset()
        {
            for (int i = 0; i < pds.Length; i++)
                pds[i] = null;

            goto_side =false;
            Move();
        }
    }

    public class control_moving :MonoBehaviour
    {
        float speed = 10;
        int status;
        Vector3 first_des;
        Vector3 second_des;


        public void set(Vector3 coast_side)
        {
            second_des = coast_side;
            if(this.transform.position.y < coast_side.y) //在岸的下面，先上再平行移动
            {
                first_des.x = this.transform.position.x;
                first_des.y = coast_side.y;
                first_des.z = coast_side.z;
                status = 1;
            }
            else if(this.transform.position.y > coast_side.y)
            {
                first_des.x = coast_side.x;
                first_des.y = this.transform.position.y;
                first_des.z = coast_side.z;
                status = 1;
            }
            else if(this.transform.position.y == coast_side.y)
            {
                status = 2;
            }
        }

        public void reset()
        {
            status = 0;
        }

        void Update()
        {
            if(status == 1)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, first_des, speed * Time.deltaTime);
                if (this.transform.position == first_des)
                    status = 2;
            }
            if(status == 2)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, second_des, speed * Time.deltaTime);
                if (this.transform.position == second_des)
                    status = 0;
            }
        }
    }
}
