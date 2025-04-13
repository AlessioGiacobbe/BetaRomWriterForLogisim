# ß-RomWriter for Logisim
  a simple program to create Logisim's rom easily.


You can download the precompiled version [here](https://github.com/AlessioGiacobbe/MicroprocessorRomWriterForLogisim/blob/master/MicroprocessorRomWriterForLogisim/precompiled/MicroprocessorRomWriterForLogisim.exe) or download the project and compile it using Visual Studio.

## How it works?
You have to write the number of instruction of your MicroProcessor and the maximum number of Micro-instructions.
<p align="center">
  <img src="https://www.dropbox.com/s/f7rtuxwi6mprz62/screen1.PNG?raw=1"/>
</p>

Then you Can select, from the combobox on the top-left, an instruction, for example `010` will select the third instruction.
Once selected the instruction that you want to edit, you can write on the boxes only the bit that you want bring to `1`.

For example if you want to write the instruction `1010` you can simply write `3 1`.
<p align="center">
  <img src="https://www.dropbox.com/s/0m4z6taw93b1i0w/screen2.PNG?raw=1"/>
</p>

Once you have wrote your rom, you can just press the export button, select your rom name and location, and save it, it will be saved in Logisim's ROM format.

On the left you can remove or add a pin, to all your micro-instructions, writing it into the box.

You can also Import existing Logisim's Roms and edit these.


 
