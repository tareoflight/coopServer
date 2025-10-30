use spacetimedb::{reducer, table, Local, ReducerContext, Table};

#[table(name = materials)]
pub struct Material {
    #[primary_key]
    id: u8,
    name: String,
    description: String,
    density: f32,
}

#[table(name = chunk_data, public, index(name = position, btree(columns = [x, y, z])))]
pub struct ChunkData {
    #[primary_key]
    #[auto_inc]
    id: u64,
    x: i32,
    y: i32,
    z: i32,
}

#[table(name = node_data, public, index(name = position, btree(columns = [chunk_id, x, y, z])))]
pub struct NodeData {
    #[primary_key]
    #[auto_inc]
    id: u64,
    chunk_id: u64, // FK ChunkData.id
    x: u8,
    y: u8,
    z: u8,
    contents: Vec<u8>,
}

#[table(name = mob, public, index(name = position, btree(columns = [pos_x, pos_y, pos_z])))]
pub struct Mob {
    #[primary_key]
    #[auto_inc]
    id: u64,
    chunk_id: u64, // FK ChunkData.id
    pos_x: f32,
    pos_y: f32,
    pos_z: f32,
    vel_azi: f32,
    vel_rot: f32,
    vel_mag: f32,
}

#[reducer]
fn make_chunk(ctx: &ReducerContext) -> Result<(), String> {
    let chunk_data = ctx.db.chunk_data();
    for _chunk in chunk_data.position().filter((0, 0, 0)) {
        return Err("Already have our one chunk".to_string());
    }
    let new_row = chunk_data.insert(ChunkData {
        id: 0,
        x: 0,
        y: 0,
        z: 0,
    });
    log::info!("Inserted new Chunk {}", new_row.id);
    Ok(())
}

#[reducer]
fn make_node(ctx: &ReducerContext) -> Result<(), String> {
    let chunk = get_chunk(&ctx.db, 0, 0, 0).or_else(|msg| Err(msg)).unwrap();
    for z in 0..=31 {
        for y in 0..=31 {
            for x in 0..=31 {
                let _ = insert_node(
                    &ctx.db,
                    NodeData {
                        id: 0,
                        chunk_id: chunk.id,
                        x,
                        y,
                        z,
                        contents: [].to_vec(),
                    },
                )
                .or_else(|msg| Err(msg));
            }
        }
    }
    Ok(())
}

fn get_chunk(db: &Local, x: i32, y: i32, z: i32) -> Result<ChunkData, String> {
    for chunk in db.chunk_data().position().filter((x, y, z)) {
        return Ok(chunk);
    }
    Err(format!("Chunk not found at ({}, {}, {})", x, y, z))
}

fn insert_node(db: &Local, node: NodeData) -> Result<u64, String> {
    for _chunk in db
        .node_data()
        .position()
        .filter((node.chunk_id, node.x, node.y, node.z))
    {
        return Err("Already a chunk at that position".to_string());
    }
    assert!(node.id == 0, "Insert node id must be 0");
    let new_row = db.node_data().insert(node);
    Ok(new_row.id)
}

#[reducer(init)]
fn fill_materials(ctx: &ReducerContext) -> Result<(), String> {
    let materials = ctx.db.materials();
    materials.insert(Material {id: 0,name: "Air".to_string(),description: "Normal breathable atmosphere".to_string(),density: 0.001});
    materials.insert(Material {id: 1, name: "Mist".to_string(), description: "Cool damp fog".to_string(), density: 0.002});
    materials.insert(Material {id: 2, name: "Smoke".to_string(), description: "Ash-laden vapor".to_string(), density: 0.004});
    materials.insert(Material {id: 3, name: "Steam".to_string(), description: "Hot water vapor".to_string(), density: 0.006});
    materials.insert(Material {id: 4, name: "Poison".to_string(), description: "Toxic fumes from alchemy".to_string(), density: 0.008});
    materials.insert(Material {id: 5, name: "Ether".to_string(), description: "Magical or spiritual vapor".to_string(), density: 0.009});
    materials.insert(Material {id: 6, name: "Spirit Fog".to_string(), description: "Ghostly ectoplasmic haze".to_string(), density: 0.01});
    materials.insert(Material {id: 7, name: "Ash Cloud".to_string(), description: "Volcanic particulate suspension".to_string(), density: 0.02});
    materials.insert(Material {id: 8, name: "Fresh Water".to_string(), description: "Clean surface water".to_string(), density: 1.0});
    materials.insert(Material {id: 9, name: "Salt Water".to_string(), description: "Oceanic brine".to_string(), density: 1.03});
    materials.insert(Material {id: 10, name: "Blood".to_string(), description: "Organic magical or natural".to_string(), density: 1.06});
    materials.insert(Material {id: 11, name: "Oil".to_string(), description: "Flammable slick liquid".to_string(), density: 0.9});
    materials.insert(Material {id: 12, name: "Acid".to_string(), description: "Corrosive alchemical liquid".to_string(), density: 1.2});
    materials.insert(Material {id: 13, name: "Quickslime".to_string(), description: "Thick magical sludge".to_string(), density: 1.4});
    materials.insert(Material {id: 14, name: "Magma".to_string(), description: "Molten rock hot and dense".to_string(), density: 2.8});
    materials.insert(Material {id: 15, name: "Liquid Metal".to_string(), description: "Molten alloy ".to_string(), density: 6.0});
    materials.insert(Material {id: 16, name: "Snow".to_string(), description: "Compressed frozen water".to_string(), density: 0.3});
    materials.insert(Material {id: 17, name: "Ice".to_string(), description: "Frozen water clear or blue".to_string(), density: 0.9});
    materials.insert(Material {id: 18, name: "Clay".to_string(), description: "Dense earthy material".to_string(), density: 1.6});
    materials.insert(Material {id: 19, name: "Dirt".to_string(), description: "Generic soil or loam".to_string(), density: 1.8});
    materials.insert(Material {id: 20, name: "Sand".to_string(), description: "Granular mineral".to_string(), density: 1.9});
    materials.insert(Material {id: 21, name: "Wood".to_string(), description: "Organic material from trees".to_string(), density: 0.8});
    materials.insert(Material {id: 22, name: "Stone".to_string(), description: "Common rock".to_string(), density: 2.6});
    materials.insert(Material {id: 23, name: "Basalt".to_string(), description: "Volcanic rock".to_string(), density: 3.0});
    materials.insert(Material {id: 24, name: "Granite".to_string(), description: "Hard crystalline rock".to_string(), density: 2.7});
    materials.insert(Material {id: 25, name: "Iron Ore".to_string(), description: "Metallic mineral".to_string(), density: 3.6});
    materials.insert(Material {id: 26, name: "Mythril Ore".to_string(), description: "Light magical metal".to_string(), density: 2.2});
    materials.insert(Material {id: 27, name: "Obsidian".to_string(), description: "Volcanic glass".to_string(), density: 2.4});
    materials.insert(Material {id: 28, name: "Silver".to_string(), description: "Precious metal".to_string(), density: 10.5});
    materials.insert(Material {id: 29, name: "Gold".to_string(), description: "Heavy precious metal".to_string(), density: 19.3});
    materials.insert(Material {id: 30, name: "Adamantite".to_string(), description: "Super-dense mythical metal".to_string(), density: 25.0});
    materials.insert(Material {id: 31, name: "Foundation".to_string(), description: "Enchanted crystalline core of the world".to_string(), density: 63.0});
    Ok(())
}
