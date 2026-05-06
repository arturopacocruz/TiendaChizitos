import { useState, useEffect } from 'react';
import axios from 'axios';
import { 
  Container, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, 
  Paper, Typography, Button, Box, TextField, Alert, Stack, Tabs, Tab, 
  MenuItem, Select, FormControl, InputLabel, IconButton, Divider, List, ListItem, ListItemText, Switch, 
  Chip,
} from '@mui/material';

// Iconos (Asegúrate de tener instalado @mui/icons-material)
import DeleteIcon from '@mui/icons-material/Delete';
import AddShoppingCartIcon from '@mui/icons-material/AddShoppingCart';
import EditIcon from '@mui/icons-material/Edit';
import SearchIcon from '@mui/icons-material/Search';

// Tipos
import type { Cliente, AñadirClienteIN, ActualizarClienteIN, BuscarParams } from './lib/types/Cliente';
import type { Categoria, AgregarCategoriaInput, ActualizarCategoriaInput } from './lib/types/Categoria';
import type { Venta, GenerarVentaInput } from './lib/types/Venta';
import type { Producto, AgregarProductoInput, ActualizarProductoInput } from './lib/types/Producto';

// Endpoints
const API_CLIENTES = 'http://localhost:5001/api/clientes';
const API_PRODUCTOS = 'http://localhost:5001/api/productos';
const API_CATEGORIAS = 'http://localhost:5001/api/categorias';
const API_VENTAS = 'http://localhost:5001/api/ventas';

function App() {
  const [tabActual, setTabActual] = useState(0);
  const [mensaje, setMensaje] = useState<{tipo: 'success' | 'error', texto: string} | null>(null);

  // --- 1. ESTADOS CLIENTES ---
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [filtrosCli, setFiltrosCli] = useState<BuscarParams>({ nombre: '', ci: undefined, extension: '' });
  const [nuevoCli, setNuevoCli] = useState<AñadirClienteIN>({ nombre: '', ci: 0, extension: '', fechaNacimiento: '' });
  const [editCli, setEditCli] = useState<(ActualizarClienteIN & { id: string }) | null>(null);

  // --- 2. ESTADOS PRODUCTOS ---
  const [productos, setProductos] = useState<Producto[]>([]);
  const [busquedaProdNombre, setBusquedaProdNombre] = useState('');
  const [filtroVigencia, setFiltroVigencia] = useState<string>('TODOS');
  const [nuevoProd, setNuevoProd] = useState<AgregarProductoInput>({ nombre: '', precio: 0, stock: 0, esVigente: true, categoriaId: '' });
  const [editProd, setEditProd] = useState<(ActualizarProductoInput & { id: string }) | null>(null);

  // --- 3. ESTADOS CATEGORÍAS ---
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [nuevaCat, setNuevaCat] = useState<AgregarCategoriaInput>({ nombre: '' });
  const [editCat, setEditCat] = useState<(ActualizarCategoriaInput & { id: string }) | null>(null);

  // --- 4. ESTADOS VENTAS ---
  const [ventas, setVentas] = useState<Venta[]>([]);
  const [ventaNueva, setVentaNueva] = useState<GenerarVentaInput>({ ci: 0, extension: '', detalle: [] });
  const [prodSeleccionado, setProdSeleccionado] = useState('');
  const [cantSeleccionada, setCantSeleccionada] = useState(1);

  useEffect(() => {
    cargarClientes(); cargarCategorias(); cargarVentas(); cargarProductos();
  }, []);

  // --- CARGAS ---
  const cargarClientes = async () => { try { const res = await axios.get(API_CLIENTES); setClientes(res.data); } catch(e){} };
  const cargarCategorias = async () => { try { const res = await axios.get(`${API_CATEGORIAS}/listarCategorias`); setCategorias(res.data); } catch(e){} };
  const cargarVentas = async () => { try { const res = await axios.get(API_VENTAS); setVentas(res.data); } catch(e){} };
  const cargarProductos = async () => {
    try {
        let url = API_PRODUCTOS;
        if (filtroVigencia !== 'TODOS') {
            url = `${API_PRODUCTOS}/ListarPorVigencia?esVigente=${filtroVigencia === 'VIGENTE'}`;
        } else if (busquedaProdNombre) {
            url = `${API_PRODUCTOS}/Buscar?nombre=${busquedaProdNombre}`;
        }
        const res = await axios.get(url);
        setProductos(res.data);
    } catch(e) {}
  };

  // --- HANDLERS ---
  const handleGuardarCliente = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editCli) await axios.put(`${API_CLIENTES}/${editCli.id}`, editCli);
      else await axios.post(`${API_CLIENTES}/Agregar`, nuevoCli);
      setEditCli(null); setNuevoCli({ nombre:'', ci:0, extension:'', fechaNacimiento:'' });
      cargarClientes(); setMensaje({ tipo: 'success', texto: "Operación exitosa" });
    } catch(e) { setMensaje({ tipo: 'error', texto: "Error" }); }
  };

  const handleGuardarProducto = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editProd) await axios.put(`${API_PRODUCTOS}/${editProd.id}`, editProd);
      else await axios.post(API_PRODUCTOS, nuevoProd);
      setEditProd(null); setNuevoProd({ nombre: '', precio: 0, stock: 0, esVigente: true, categoriaId: '' });
      cargarProductos(); setMensaje({ tipo: 'success', texto: "Producto guardado" });
    } catch(e) {}
  };

  const ejecutarVenta = async () => {
    try {
      await axios.post(API_VENTAS, ventaNueva);
      setVentaNueva({ ci: 0, extension: '', detalle: [] });
      cargarVentas(); cargarProductos();
      setMensaje({ tipo: 'success', texto: "Venta realizada con éxito" });
    } catch(err: any) { setMensaje({ tipo: 'error', texto: err.response?.data || "Error al vender" }); }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 3 }}>
      <Typography variant="h4" align="center" sx={{ mb: 3, fontWeight: 'bold', color: '#1976d2' }}>ERP TIENDA CHIZITOS</Typography>

      <Paper sx={{ mb: 4 }}>
        <Tabs value={tabActual} onChange={(_, v) => setTabActual(v)} variant="fullWidth">
          <Tab label="1. Clientes" />
          <Tab label="2. Productos" />
          <Tab label="3. Categorías" />
          <Tab label="4. Ventas" />
        </Tabs>
      </Paper>

      {mensaje && <Alert severity={mensaje.tipo} sx={{ mb: 2 }} onClose={() => setMensaje(null)}>{mensaje.texto}</Alert>}

      {/* --- 1. SECCIÓN CLIENTES --- */}
      {tabActual === 0 && (
        <Box>
           <Box sx={{ display: 'flex', gap: 2, mb: 4, flexDirection: { xs: 'column', md: 'row' } }}>
            <Paper sx={{ p: 3, flex: 1, bgcolor: '#f1f8e9' }}>
              <Typography variant="h6" gutterBottom color="success.main">Registro</Typography>
              <Box component="form" onSubmit={handleGuardarCliente}>
                <Stack spacing={2}>
                  <TextField label="Nombre" size="small" required value={editCli ? editCli.nombre : nuevoCli.nombre} onChange={e => editCli ? setEditCli({...editCli, nombre: e.target.value}) : setNuevoCli({...nuevoCli, nombre: e.target.value})} />
                  <Stack direction="row" spacing={1}>
                    <TextField label="CI" type="number" size="small" required value={editCli ? editCli.ci : (nuevoCli.ci || '')} onChange={e => editCli ? setEditCli({...editCli, ci: parseInt(e.target.value) || 0}) : setNuevoCli({...nuevoCli, ci: parseInt(e.target.value) || 0})} />
                    <TextField label="Ext." size="small" sx={{ width: 80 }} value={editCli ? editCli.extension : nuevoCli.extension} onChange={e => editCli ? setEditCli({...editCli, extension: e.target.value}) : setNuevoCli({...nuevoCli, extension: e.target.value})} />
                  </Stack>
                  <TextField label="F. Nacimiento" type="date" size="small" slotProps={{ inputLabel: { shrink: true } }} required value={editCli ? editCli.fechaNacimiento : nuevoCli.fechaNacimiento} onChange={e => editCli ? setEditCli({...editCli, fechaNacimiento: e.target.value}) : setNuevoCli({...nuevoCli, fechaNacimiento: e.target.value})} />
                  <Button type="submit" variant="contained" color={editCli ? "warning" : "success"}>Guardar</Button>
                </Stack>
              </Box>
            </Paper>
            <Paper sx={{ p: 3, flex: 1, bgcolor: '#e3f2fd' }}>
              <Typography variant="h6" gutterBottom color="primary.main">Buscador</Typography>
              <Stack spacing={2}>
                <TextField label="Nombre" size="small" value={filtrosCli.nombre} onChange={e => setFiltrosCli({...filtrosCli, nombre: e.target.value})} />
                <Stack direction="row" spacing={1}>
                    <TextField label="CI" type="number" size="small" fullWidth value={filtrosCli.ci || ''} onChange={e => setFiltrosCli({...filtrosCli, ci: parseInt(e.target.value) || undefined})} />
                    <TextField label="Ext." size="small" sx={{ width: 100 }} value={filtrosCli.extension} onChange={e => setFiltrosCli({...filtrosCli, extension: e.target.value})} />
                </Stack>
                <Button variant="contained" startIcon={<SearchIcon />} onClick={async () => {
                  const p = new URLSearchParams();
                  if (filtrosCli.nombre) p.append('nombre', filtrosCli.nombre);
                  if (filtrosCli.ci) p.append('ci', filtrosCli.ci.toString());
                  if (filtrosCli.extension) p.append('extension', filtrosCli.extension);
                  const res = await axios.get(`${API_CLIENTES}/Buscar?${p.toString()}`); setClientes(res.data);
                }}>Filtrar</Button>
              </Stack>
            </Paper>
          </Box>
          <TableContainer component={Paper}><Table size="small"><TableHead sx={{ bgcolor: '#2e7d32' }}><TableRow><TableCell sx={{ color:'white' }}>CI / Ext</TableCell><TableCell sx={{ color:'white' }}>Nombre</TableCell><TableCell sx={{ color:'white' }}>Acciones</TableCell></TableRow></TableHead>
          <TableBody>{clientes.map(c => <TableRow key={c.id} hover><TableCell>{c.ci} {c.extension}</TableCell><TableCell>{c.nombre}</TableCell>
          <TableCell><IconButton color="primary" onClick={() => setEditCli({ id: c.id, nombre: c.nombre, ci: c.ci, extension: c.extension || '', fechaNacimiento: c.fechaNacimiento.split('T')[0] })}><EditIcon /></IconButton></TableCell></TableRow>)}</TableBody></Table></TableContainer>
        </Box>
      )}

      {/* --- 2. SECCIÓN PRODUCTOS --- */}
      {/* 2. PRODUCTOS */}
{tabActual === 1 && (
  <Box>
    <Paper sx={{ p: 2, mb: 3, bgcolor: '#f5f5f5' }}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <TextField label="Buscar por nombre" size="small" value={busquedaProdNombre} onChange={e => setBusquedaProdNombre(e.target.value)} />
          <FormControl size="small" sx={{ minWidth: 150 }}>
              <InputLabel>Vigencia</InputLabel>
              <Select value={filtroVigencia} label="Vigencia" onChange={e => setFiltroVigencia(e.target.value)}>
                  <MenuItem value="TODOS">Todos</MenuItem>
                  <MenuItem value="VIGENTE">Vigentes</MenuItem>
                  <MenuItem value="NO_VIGENTE">No Vigentes</MenuItem>
              </Select>
          </FormControl>
          <Button variant="contained" onClick={cargarProductos} startIcon={<SearchIcon />}>Ok</Button>
      </Stack>
    </Paper>

    <Box component="form" onSubmit={handleGuardarProducto}>
      <Paper sx={{ p: 3, mb: 3, bgcolor: '#fbe9e7' }}>
        <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap', gap: 2, alignItems: 'center' }}>
          <TextField label="Nombre" size="small" required value={editProd ? editProd.nombre : nuevoProd.nombre} onChange={e => editProd ? setEditProd({...editProd, nombre: e.target.value}) : setNuevoProd({...nuevoProd, nombre: e.target.value})} />
          <TextField label="Precio" type="number" size="small" required value={editProd ? editProd.precio : nuevoProd.precio} onChange={e => editProd ? setEditProd({...editProd, precio: parseFloat(e.target.value)}) : setNuevoProd({...nuevoProd, precio: parseFloat(e.target.value)})} />
          <TextField label="Stock" type="number" size="small" required value={editProd ? editProd.stock : nuevoProd.stock} onChange={e => editProd ? setEditProd({...editProd, stock: parseInt(e.target.value)}) : setNuevoProd({...nuevoProd, stock: parseInt(e.target.value)})} />
          
          {/* EL "TOGGLE" (SWITCH) DE MATERIAL UI */}
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <Typography variant="body2">¿Vigente?</Typography>
            <Switch 
              checked={editProd ? editProd.esVigente : nuevoProd.esVigente}
              onChange={e => {
                const val = e.target.checked;
                editProd ? setEditProd({...editProd, esVigente: val}) : setNuevoProd({...nuevoProd, esVigente: val})
              }}
              color="success"
            />
          </Box>

          <FormControl size="small" sx={{ minWidth: 150 }}><InputLabel>Cat</InputLabel>
            <Select value={editProd ? (editProd.categoriaId || '') : (nuevoProd.categoriaId || '')} label="Cat" onChange={e => editProd ? setEditProd({...editProd, categoriaId: e.target.value}) : setNuevoProd({...nuevoProd, categoriaId: e.target.value})}>
              {categorias.map(cat => <MenuItem key={cat.id} value={cat.id}>{cat.nombre}</MenuItem>)}
            </Select>
          </FormControl>
          <Button type="submit" variant="contained" color="error">{editProd ? "Update" : "Save"}</Button>
        </Stack>
      </Paper>
    </Box>

    <TableContainer component={Paper}>
      <Table size="small">
        <TableHead sx={{ bgcolor: '#d32f2f' }}>
          <TableRow>
            <TableCell sx={{ color:'white' }}>Producto</TableCell>
            <TableCell sx={{ color:'white' }}>Precio</TableCell>
            <TableCell sx={{ color:'white' }}>Stock</TableCell>
            <TableCell sx={{ color:'white' }}>Vigente</TableCell>
            <TableCell sx={{ color:'white' }}>Acciones</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {productos.map(p => (
            <TableRow key={p.id} hover>
              <TableCell>{p.nombre}</TableCell>
              <TableCell>Bs. {p.precio}</TableCell>
              <TableCell>{p.stock}</TableCell>
              <TableCell>
                {/* Visualización profesional con Chip */}
                <Chip 
                  label={p.esVigente ? "Activo" : "Inactivo"} 
                  color={p.esVigente ? "success" : "default"} 
                  size="small" 
                />
              </TableCell>
              <TableCell>
                <IconButton onClick={() => setEditProd({ id: p.id, nombre: p.nombre, precio: p.precio, stock: p.stock, esVigente: p.esVigente, categoriaId: p.categoriaId || '' })}><EditIcon /></IconButton>
                <IconButton color="error" onClick={async () => { if(confirm("¿Eliminar?")) { await axios.delete(`${API_PRODUCTOS}/${p.id}`); cargarProductos(); } }}><DeleteIcon /></IconButton>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  </Box>
)}
      {/* --- 3. SECCIÓN CATEGORÍAS --- */}
      {tabActual === 2 && (
        <Box>
           <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
            <Paper sx={{ p: 2, flex: 1, bgcolor: '#e3f2fd', display: 'flex', gap: 1 }}>
              <TextField label="Nombre" size="small" onChange={async (e) => { const res = await axios.get(`${API_CATEGORIAS}/BuscarCategoria?nombre=${e.target.value}`); setCategorias(res.data); }} />
              <Button variant="contained">Search</Button>
            </Paper>
            <Box component="form" onSubmit={handleGuardarProducto} sx={{ flex: 1 }}>
              <Paper sx={{ p: 2, bgcolor: '#fff3e0', display: 'flex', gap: 1 }}>
                <TextField label="Categoría" size="small" required value={editCat ? editCat.nombre : nuevaCat.nombre} onChange={e => editCat ? setEditCat({...editCat, nombre: e.target.value}) : setNuevaCat({nombre: e.target.value})} />
                <Button type="submit" variant="contained" color="warning">{editCat ? "U" : "+"}</Button>
              </Paper>
            </Box>
          </Stack>
          <TableContainer component={Paper}><Table size="small"><TableHead sx={{ bgcolor: '#1565c0' }}><TableRow><TableCell sx={{ color:'white' }}>Nombre</TableCell><TableCell sx={{ color:'white' }}>Acciones</TableCell></TableRow></TableHead>
          <TableBody>{categorias.map(cat => <TableRow key={cat.id} hover><TableCell>{cat.nombre}</TableCell><TableCell><Button size="small" onClick={() => setEditCat({ id: cat.id, nombre: cat.nombre })}>Edit</Button></TableCell></TableRow>)}</TableBody></Table></TableContainer>
        </Box>
      )}

      {/* --- 4. SECCIÓN VENTAS --- */}
      {tabActual === 3 && (
        <Box sx={{ display: 'flex', gap: 3, flexDirection: { xs: 'column', md: 'row' } }}>
          <Box sx={{ flex: 1 }}>
            <Paper sx={{ p: 3, bgcolor: '#f3e5f5' }}>
              <Typography variant="h6" color="secondary" gutterBottom>Nueva Venta</Typography>
              <Stack spacing={2}>
                {/* CI Y EXTENSIÓN AÑADIDOS AQUÍ */}
                <Stack direction="row" spacing={1}>
                  <TextField 
                    label="CI Cliente" type="number" size="small" fullWidth 
                    value={ventaNueva.ci || ''} 
                    onChange={e => setVentaNueva({...ventaNueva, ci: parseInt(e.target.value) || 0})} 
                  />
                  <TextField 
                    label="Ext." size="small" sx={{ width: 100 }} 
                    value={ventaNueva.extension} 
                    onChange={e => setVentaNueva({...ventaNueva, extension: e.target.value})} 
                  />
                </Stack>

                <FormControl fullWidth size="small"><InputLabel>Producto</InputLabel>
                  <Select value={prodSeleccionado} label="Producto" onChange={e => setProdSeleccionado(e.target.value)}>
                    {productos.map(p => <MenuItem key={p.id} value={p.id}>{p.nombre} (Bs. {p.precio})</MenuItem>)}
                  </Select>
                </FormControl>
                
                <Stack direction="row" spacing={1}>
                  <TextField label="Cant." type="number" size="small" sx={{ width: 80 }} value={cantSeleccionada} onChange={e => setCantSeleccionada(parseInt(e.target.value) || 1)} />
                  <Button variant="contained" color="secondary" fullWidth startIcon={<AddShoppingCartIcon />} onClick={() => {
                    if (!prodSeleccionado) return;
                    const ex = ventaNueva.detalle.find(d => d.productoId === prodSeleccionado);
                    if (ex) setVentaNueva({ ...ventaNueva, detalle: ventaNueva.detalle.map(d => d.productoId === prodSeleccionado ? { ...d, cantidad: d.cantidad + cantSeleccionada } : d) });
                    else setVentaNueva({ ...ventaNueva, detalle: [...ventaNueva.detalle, { productoId: prodSeleccionado, cantidad: cantSeleccionada }] });
                    setProdSeleccionado(''); setCantSeleccionada(1);
                  }}>Add</Button>
                </Stack>
                <Divider />
                <List dense sx={{ maxHeight: 200, overflow: 'auto' }}>
                  {ventaNueva.detalle.map((it, i) => (
                    <ListItem key={i} secondaryAction={<IconButton edge="end" onClick={() => setVentaNueva({...ventaNueva, detalle: ventaNueva.detalle.filter(d => d.productoId !== it.productoId)}) }><DeleteIcon color="error"/></IconButton>}>
                      <ListItemText primary={productos.find(p => p.id === it.productoId)?.nombre} secondary={`Cant: ${it.cantidad}`} />
                    </ListItem>
                  ))}
                </List>
                {ventaNueva.detalle.length > 0 && <Button variant="contained" color="success" fullWidth onClick={ejecutarVenta}>Confirmar Venta</Button>}
              </Stack>
            </Paper>
          </Box>
          <Box sx={{ flex: 1.5 }}>
            <TableContainer component={Paper}><Table size="small"><TableHead sx={{ bgcolor: '#673ab7' }}><TableRow><TableCell sx={{ color:'white' }}>Fecha</TableCell><TableCell sx={{ color:'white' }}>Cliente</TableCell><TableCell sx={{ color:'white' }}>Total</TableCell></TableRow></TableHead>
            <TableBody>{ventas.map(v => <TableRow key={v.id} hover><TableCell>{new Date(v.fecha).toLocaleDateString()}</TableCell><TableCell>{v.cliente.nombre}</TableCell><TableCell sx={{ fontWeight: 'bold' }}>Bs. {v.total}</TableCell></TableRow>)}</TableBody></Table></TableContainer>
          </Box>
        </Box>
      )}
    </Container>
  );
}

export default App; 