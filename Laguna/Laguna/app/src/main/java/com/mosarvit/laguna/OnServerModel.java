package com.mosarvit.laguna;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;

/**
 * Created by Mosarvit on 11/3/2017.
 */

public abstract class OnServerModel extends Model {

    @Column(name = "remote_id", unique = true, onUniqueConflict = Column.ConflictAction.REPLACE)
    public int remote_id;

    @Column(name = "updatetime")
    public long updatetime;

    @Column(name = "utwhenloaded")
    public long utwhenloaded;

    @Column(name = "toDelete")
    public boolean toDelete;

    @Column(name = "isNew")
    public boolean isNew;

}
